using System;
using System.Net;

namespace Bet.AspNetCore.EventGrid.MessageHandlers
{
    public class CloudEventClientException : Exception
    {
        public CloudEventClientException(HttpStatusCode statusCode, string message) : base(message)
        {
            StatusCode = statusCode;
        }

        public HttpStatusCode StatusCode { get; }
    }
}
