using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HeartRate.API.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace HeartRate.API
{
    public class StorageTableManager
    {
        private CloudTable _table;
        
        public IConfiguration Configuration { get; }
        
        public StorageTableManager(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void Connect()
        {
            var connectionString = Configuration["StorageTableConnectionString"];
//            var connectionString = "DefaultEndpointsProtocol=https;AccountName=beatstable;AccountKey=AlzJqi16Wz2X/s/qm303+0q53mDFDyF3/8/D69W4UeSolw2Y4LnoY3U9DRUwtxk/0xiC07wbTBUXP9Aip/9kuw==;EndpointSuffix=core.windows.net";
            var storageAccount = CloudStorageAccount.Parse(connectionString);
            var tableClient = storageAccount.CreateCloudTableClient();
            _table = tableClient.GetTableReference("heartbeats");
        }

        public async Task AddEntity(ReceivedData receivedData)
        {
            foreach (var data in receivedData.Data)
            {
                var pKey = DateTime.ParseExact(data.Key, "yyyyMMdd_HHmmss_fff", null).Subtract(new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds*1000;
                var beatsEntity = new BeatEntity(receivedData.UserId, pKey.ToString())
                {
                    DateTime = DateTime.ParseExact(data.Key, "yyyyMMdd_HHmmss_fff", null),
                    HeartRate = data.Value
                };
                var insertOperation = TableOperation.Insert(beatsEntity);
                await _table.ExecuteAsync(insertOperation);
            }
        }

        public async Task<QueryData> RetrieveRangeData(string userId, string start, string end)
        {
            var rangeQuery = new TableQuery<BeatEntity>().Where(
                TableQuery.CombineFilters(
                    TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, userId),
                    TableOperators.And,
                    TableQuery.CombineFilters(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.GreaterThanOrEqual, start),
                    TableOperators.And,
                    TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.LessThanOrEqual, end))
                )
            );
            
            var beatsEntities = new List<BeatEntity>();
            TableContinuationToken token = null;
            do
            {
                var item = await _table.ExecuteQuerySegmentedAsync(rangeQuery, token);
                beatsEntities.AddRange(item.Results);
                token = item.ContinuationToken;
            } while (token != null);
            
            var queryData = new QueryData
            {
                UserId = userId,
                Data = new SortedList<string, int>(new DuplicateKeyComparer<string>())
            };
            
            foreach (var entity in beatsEntities)
            {
                queryData.Data.Add(entity.PartitionKey, entity.HeartRate);
            }
            return queryData;
        }

        public List<List<double>> DictToArray(QueryData queryData)
        {
            var res = new List<List<double>>();
            foreach (var item in queryData.Data)
            {
                var list = new List<double> {double.Parse(item.Key), item.Value};
                res.Add(list);
            }
            return res;
        }
    }
    
    public class BeatEntity : TableEntity
    {
        public BeatEntity(){}
        
        public BeatEntity(string userId, string dateTime)
        {
            PartitionKey = dateTime;
            RowKey = userId;
        }

        public DateTime DateTime { get; set; }
        
        public int HeartRate { get; set; }
    }
}