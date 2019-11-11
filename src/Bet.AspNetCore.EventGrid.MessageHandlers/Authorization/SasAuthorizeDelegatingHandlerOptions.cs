using System;

namespace Bet.AspNetCore.EventGrid.MessageHandlers.Authorization
{
    /// <summary>
    /// Options configurations for the <see cref="SasAuthorizeDelegatingHandler"/>.
    /// </summary>
    public class SasAuthorizeDelegatingHandlerOptions
    {
        public SasAuthorizeDelegatingHandlerOptions()
        {
        }

        public SasAuthorizeDelegatingHandlerOptions(string endpoint, string key, TimeSpan? tokenExpiration = null)
        {
            Endpoint = endpoint;
            Key = key;
            if (tokenExpiration.HasValue)
            {
                TokenExpiration = tokenExpiration.Value;
            }
        }

        /// <summary>
        /// The Event Grid base address.
        /// </summary>
        public string Endpoint { get; set; } = string.Empty;

        /// <summary>
        /// The Event Grid Authentication Key.
        /// </summary>
        public string Key { get; set; } = string.Empty;

        /// <summary>
        /// The Expiration for the SAS Token. The default is 15 minutes.
        /// </summary>
        public TimeSpan TokenExpiration { get; set; } = TimeSpan.FromMinutes(15);
    }
}
