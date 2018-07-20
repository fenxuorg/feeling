using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace SmartYi.WebApi.Handlers
{
    /// <summary>
    /// The global diagnostics handler.
    /// </summary>
    public class GlobalDiagnosticsHandler
    {
        private readonly RequestDelegate next;

        /// <summary>
        /// Initializes a new instance of the <see cref="GlobalDiagnosticsHandler"/> class.
        /// The constructor of handler.
        /// </summary>
        /// <param name="next">The next delegate</param>
        public GlobalDiagnosticsHandler(RequestDelegate next)
        {
            this.next = next;
        }

        /// <summary>
        /// The invoke method.
        /// </summary>
        /// <param name="context">The http context.</param>
        /// <returns>The return task</returns>
        public async Task Invoke(HttpContext context)
        {
            await next(context);
        }
    }
}
