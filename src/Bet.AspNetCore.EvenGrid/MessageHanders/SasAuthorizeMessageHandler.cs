using System;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Bet.AspNetCore.EvenGrid.MessageHanders
{
    public class SasAuthorizeMessageHandler : DelegatingHandler
    {
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1);

        private const string _sasHeaderKey = "aeg-sas-token";

        private readonly SasAuthorizeOptions _options;
        private string _sasToken;

        public SasAuthorizeMessageHandler(SasAuthorizeOptions options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (!request.Headers.Contains(_sasHeaderKey) && string.IsNullOrEmpty(_sasToken))
            {
                _sasToken = BuildSharedAccessSignature(
                    $"{_options.Endpoint}?api-version=2018-01-01",
                    DateTime.UtcNow + _options.TokenExpiration,
                    _options.Key,
                    cancellationToken);
            }

            request.Headers.Add(_sasHeaderKey, _sasToken);

            var response = await base.SendAsync(request, cancellationToken);

            if (response.StatusCode != HttpStatusCode.Unauthorized)
            {
                return response;
            }

            {
                request.Headers.Remove(_sasHeaderKey);

                _sasToken = BuildSharedAccessSignature(
                    $"{_options.Endpoint}?api-version=2018-01-01",
                    DateTime.UtcNow + _options.TokenExpiration,
                    _options.Key,
                    cancellationToken);

                request.Headers.Add(_sasHeaderKey, _sasToken);

                response = await base.SendAsync(request, cancellationToken);
            }

            return response;
        }

        private string BuildSharedAccessSignature(
            string resource,
            DateTime expirationUtc,
            string key,
            CancellationToken cancellationToken)
        {
            try
            {
                _semaphore.Wait(cancellationToken);

                const char Resource = 'r';
                const char Expiration = 'e';
                const char Signature = 's';

                var encodedResource = HttpUtility.UrlEncode(resource);
                var culture = CultureInfo.CreateSpecificCulture("en-US");
                var encodedExpirationUtc = HttpUtility.UrlEncode(expirationUtc.ToString(culture));

                var unsignedSas = $"{Resource}={encodedResource}&{Expiration}={encodedExpirationUtc}";
                using (var hmac = new HMACSHA256(Convert.FromBase64String(key)))
                {
                    var signature = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(unsignedSas)));
                    var encodedSignature = HttpUtility.UrlEncode(signature);
                    var signedSas = $"{unsignedSas}&{Signature}={encodedSignature}";

                    return signedSas;
                }
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }
}
