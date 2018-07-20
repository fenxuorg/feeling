using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Tokens;

namespace SmartYi.WebApi.Handlers
{
    /// <summary>
    /// The class of token authentication options.
    /// </summary>
    public class TokenAuthenticationOptions : AuthenticationSchemeOptions
    {
        /// <summary>
        /// Gets or sets the identity
        /// </summary>
        public ClaimsIdentity Identity { get; set; }

        /// <summary>
        /// Gets the SecurityTokenValidator
        /// </summary>
        public ISecurityTokenValidator SecurityTokenValidator { get; } = new TokenValidator();
    }
}
