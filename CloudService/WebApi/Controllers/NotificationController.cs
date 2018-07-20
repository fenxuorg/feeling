using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace SmartYi.WebApi.Controllers
{
    /// <summary>
    /// The notification controller.
    /// </summary>
    [ApiVersion("1.0")]
    [Route("api/v{api-version:apiVersion}/[controller]")]
    [Authorize(Policy = "Member")]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class NotificationController : Controller
    {
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationController"/> class.
        /// </summary>
        /// <param name="configuration">The configuration</param>
        public NotificationController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Send event method.
        /// </summary>
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public Task SendEventAsync()
        {
            throw new NotImplementedException();
        }
    }
}