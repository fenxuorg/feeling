namespace SmartYi.WebApi
{
    /// <summary>
    /// The constants.
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// The default schema name.
        /// </summary>
        public const string DefaultSchemeName = "Bearer";

        /// <summary>
        /// The scheme name of api key.
        /// </summary>
        public const string ApiKeySchemeName = "Authorization";

        /// <summary>
        /// The scheme description of api key.
        /// </summary>
        public const string ApiKeySchemeDescription = "Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"";
    }
}
