using System;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace SmartYi.WebApi.Handlers
{
    /// <summary>
    /// The token authentication handler.
    /// </summary>
    public class TokenAuthenticationHandler : AuthenticationHandler<TokenAuthenticationOptions>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TokenAuthenticationHandler"/> class.
        /// The constructor of token authentication handler.
        /// </summary>
        /// <param name="options">The token authentication options.</param>
        /// <param name="logger">The logger factory.</param>
        /// <param name="encoder">The url encoder.</param>
        /// <param name="clock">The system clock.</param>
        public TokenAuthenticationHandler(IOptionsMonitor<TokenAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
        }

        /// <summary>
        /// The method of handle authentication
        /// </summary>
        /// <returns>The task of authenticate result</returns>
        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            string authorizationHeader = Request.Headers[Constants.ApiKeySchemeName];
            if (string.IsNullOrEmpty(authorizationHeader))
            {
                return Task.FromResult(AuthenticateResult.NoResult());
            }

            if (!authorizationHeader.StartsWith(Constants.DefaultSchemeName + ' ', StringComparison.OrdinalIgnoreCase))
            {
                return Task.FromResult(AuthenticateResult.NoResult());
            }

            string token = authorizationHeader.Substring(Constants.DefaultSchemeName.Length).Trim();
            if (string.IsNullOrEmpty(token))
            {
                return Task.FromResult(AuthenticateResult.NoResult());
            }
            SecurityToken validatedToken;
            if (Options.SecurityTokenValidator != null)
            {
                var principal = Options.SecurityTokenValidator.ValidateToken(token, new TokenValidationParameters(), out validatedToken);
                var ticket = new AuthenticationTicket(principal, new AuthenticationProperties(), this.Scheme.Name);
                return Task.FromResult(AuthenticateResult.Success(ticket));
            }
            return Task.FromResult(AuthenticateResult.Success(null));
        }
    }
}
