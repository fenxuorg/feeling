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
        private readonly TableManager _tableManager;

        public HeartrateController(BlobManager blobManager, TableManager tableManager)
        {
            _blobManager = blobManager;
            _tableManager = tableManager;
        }

        [HttpGet]
        [Route("query")]
        public async Task<IActionResult> Get(string userId, string start, string end)
        {
            var queryResult = await _tableManager.RetrieveRangeData(userId, start, end);
            return Ok(queryResult);
        }

        [HttpPost]
        [Route("receive")]
        public async Task<IActionResult> Post([FromBody]ReceivedData receivedData)
        {
            await _blobManager.UploadData(receivedData);
            await _tableManager.AddEntity(receivedData);
            return Ok("Received");
        }
    }
}