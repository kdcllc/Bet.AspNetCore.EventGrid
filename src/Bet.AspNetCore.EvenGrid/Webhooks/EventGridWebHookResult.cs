using System;

namespace Bet.AspNetCore.EvenGrid.Webhooks
{
    /// <summary>
    /// The result message for the custom event grid webhook processors.
    /// </summary>
    public class EventGridWebHookResult
    {
        public EventGridWebHookResult(Exception exception = null)
        {
            Exception = exception;
        }

        /// <summary>
        /// Gets an <see cref="Exception"/> representing the exception that was thrown when checking for status (if any).
        /// </summary>
        public Exception Exception { get; }
    }
}
