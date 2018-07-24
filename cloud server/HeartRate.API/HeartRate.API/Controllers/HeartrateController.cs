using System.Threading.Tasks;
using HeartRate.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace HeartRate.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HeartrateController : Controller
    {
        private readonly BlobManager _blobManager;
        private readonly CosmosTableManager _cosmosTableManager;
        private readonly StorageTableManager _storageTableManager;

        public HeartrateController(BlobManager blobManager, CosmosTableManager cosmosTableManager, StorageTableManager storageTableManager)
        {
            _blobManager = blobManager;
            _cosmosTableManager = cosmosTableManager;
            _storageTableManager = storageTableManager;
        }

        [HttpGet]
        [Route("query")]
        public async Task<IActionResult> Get(string userId, string start, string end)
        {
//            var queryResult = await _cosmosTableManager.RetrieveRangeData(userId, start, end);
            var queryResult = await _storageTableManager.RetrieveRangeData(userId, start, end);
            var result = _storageTableManager.DictToArray(queryResult);
            return Ok(result);
        }

        [HttpPost]
        [Route("receive")]
        public async Task<IActionResult> Post([FromBody]ReceivedData receivedData)
        {
            await _blobManager.UploadData(receivedData);
//            await _cosmosTableManager.AddEntity(receivedData);
            await _storageTableManager.AddEntity(receivedData);
            return Ok("Received");
        }
    }
}