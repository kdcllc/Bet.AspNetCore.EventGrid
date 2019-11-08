using System;
using System.Net;

namespace Bet.AspNetCore.EventGrid.MessageHandlers
{
    public class EventGridHttpClientException : Exception
    {
        public EventGridHttpClientException(HttpStatusCode statusCode, string message) : base(message)
        {
            StatusCode = statusCode;
        }

        public HttpStatusCode StatusCode { get; }
    }
}
