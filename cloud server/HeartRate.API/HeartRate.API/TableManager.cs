using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HeartRate.API.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace HeartRate.API
{
    public class TableManager
    {
        private CloudTable _table;
        
        public IConfiguration Configuration { get; }
        
        public TableManager(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void Connect()
        {
//            var connectionString = Configuration["TableConnectionString"];
            var connectionString = "DefaultEndpointsProtocol=https;AccountName=feelingheartrate;AccountKey=qP5t7mwqHfe7cTTpV73WRPt6ZZVoAO75J1GVjb7k89LwOrxYKU42xWrZq6u7ukbc8HCdoQml3H5m0KcS8aPGfg==;TableEndpoint=https://feelingheartrate.table.cosmosdb.azure.com:443/;";
            var storageAccount = CloudStorageAccount.Parse(connectionString);
            var tableClient = storageAccount.CreateCloudTableClient();
            _table = tableClient.GetTableReference("heartrate");
        }

        public async Task AddEntity(ReceivedData receivedData)
        {
            foreach (var data in receivedData.Data)
            {
                var beatsEntity = new BeatsEntity(receivedData.UserId, data.Key) {HeartRate = data.Value};
                var insertOperation = TableOperation.Insert(beatsEntity);
                await _table.ExecuteAsync(insertOperation);
            }
        }

        public async Task<QueryData> RetrieveRangeData(string userId, string start, string end)
        {
            var rangeQuery = new TableQuery<BeatsEntity>().Where(
                TableQuery.CombineFilters(
                    TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, userId),
                    TableOperators.And,
                    TableQuery.CombineFilters(TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.GreaterThanOrEqual, start),
                    TableOperators.And,
                    TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.LessThanOrEqual, end))
                )
            );
            
            var beatsEntities = new List<BeatsEntity>();
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
                queryData.Data.Add(entity.RowKey, entity.HeartRate);
            }
            return queryData;
        }
    }
    
    public class BeatsEntity : TableEntity
    {
        public BeatsEntity(){}
        
        public BeatsEntity(string userId, string dateTime)
        {
            PartitionKey = userId;
            RowKey = dateTime;
        }
            
        public int HeartRate { get; set; }
    }
    
    public class DuplicateKeyComparer<TKey> : IComparer<TKey> 
        where TKey : IComparable
    {
        #region IComparer<TKey> Members

        public int Compare(TKey x, TKey y)
        {
            var result = x.CompareTo(y);
            return result == 0 ? 1 : result;
        }

        #endregion
    }
}