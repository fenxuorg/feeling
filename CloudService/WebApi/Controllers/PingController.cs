using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SmartYi.WebApi.Controllers
{
    /// <summary>
    /// The test controller
    /// </summary>
    [ApiVersion("1.0")]
    [Route("api/v{api-version:apiVersion}/[controller]")]
    [Produces("application/json")]
    [Consumes("application/json")]
    [AllowAnonymous]
    public class PingController : Controller
    {
        /// <summary>
        /// The test method.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        public Task<string> Ping()
        {
            return Task.FromResult("Pong");
        }
    }
}
