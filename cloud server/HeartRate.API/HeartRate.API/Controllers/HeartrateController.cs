using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using HeartRate.API.Models;

namespace HeartRate.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HeartrateController : Controller
    {
        private readonly BlobManager _blobManager;

        public HeartrateController(BlobManager blobManager)
        {
            _blobManager = blobManager;
        }

        [HttpGet]
        [Route("query")]
        public ActionResult<Models.HeartRate> Get(string userId, DateTime start, DateTime end)
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        [Route("receive")]
        public async Task<IActionResult> Post([FromBody]Models.HeartRate heartRate)
        {
            await _blobManager.UploadData(heartRate);
            return Ok("Received");
        }
    }
}