using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace SmartYi.WebApi.Handlers
{
    /// <summary>
    /// A <see cref="TokenValidator"/> designed for creating and validating Web Tokens.
    /// </summary>
    public class TokenValidator : ISecurityTokenValidator
    {
        /// <summary>
        /// Gets a value indicating whether this handler supports validation of tokens handled by this instance.
        /// </summary>
        /// <returns>'True' if the instance is capable of SecurityToken validation.</returns>
        public bool CanValidateToken => true;

        /// <summary>
        /// Gets or sets the maximum token size in bytes
        /// </summary>v
        public int MaximumTokenSizeInBytes { get; set; } = TokenValidationParameters.DefaultMaximumTokenSizeInBytes;

        /// <summary>
        /// Indicates whether the current token string can be read as a token of the type handled by this instance.
        /// </summary>
        /// <param name="securityToken">The token string thats needs to be read.</param>
        /// <returns>'True' if the ReadToken method can parse the token string.</returns>
        public bool CanReadToken(string securityToken)
        {
            return true;
        }

        /// <summary>
        /// Reads and validates a 'Web Token'.
        /// </summary>
        /// <param name="securityToken">the security token.</param>
        /// <param name="validationParameters">Contains validation parameters for the <see cref="SecurityToken"/>.</param>
        /// <param name="validatedToken">The <see cref="SecurityToken"/> that was validated.</param>
        /// <returns> A <see cref="ClaimsPrincipal"/> the principal after validate.</returns>
        public ClaimsPrincipal ValidateToken(string securityToken, TokenValidationParameters validationParameters, out SecurityToken validatedToken)
        {
            // todo: to call service to get the permission.
            var principle = new ClaimsPrincipal();
            var claims = new[] { new Claim("token", securityToken) };
            principle.AddIdentity(new ClaimsIdentity(claims, nameof(TokenAuthenticationHandler)));
            validatedToken = null;
            return principle;
        }
    }
}
