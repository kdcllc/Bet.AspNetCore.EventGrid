using System;

namespace Bet.AspNetCore.EvenGrid.MessageHanders
{
    /// <summary>
    /// Options configurations for the <see cref="SasAuthorizeMessageHandler"/>.
    /// </summary>
    public class SasAuthorizeOptions
    {
        public SasAuthorizeOptions()
        {
        }

        public SasAuthorizeOptions(string endpoint, string key, TimeSpan? tokenExpiration = null)
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
        public string Endpoint { get; set; }

        /// <summary>
        /// The Event Grid Authentication Key.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// The Expiration for the SAS Token. The default is 15 minutes.
        /// </summary>
        public TimeSpan TokenExpiration { get; set; } = TimeSpan.FromMinutes(15);
    }
}
