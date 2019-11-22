namespace Bet.AspNetCore.EventGrid.Diagnostics
{
    /// <summary>
    /// The Process Reporting Service.
    /// </summary>
    public interface IProcessReport
    {
        /// <summary>
        /// The Failure count.
        /// </summary>
        int FailureCount { get; }

        /// <summary>
        /// The Success Count.
        /// </summary>
        /// <returns></returns>
        int SuccessCount { get; }

        /// <summary>
        /// To Increment Success.
        /// </summary>
        int IncrementSuccess();

        /// <summary>
        /// To Increment Failure.
        /// </summary>
        int IncrementFailure();

        /// <summary>
        /// Resets the current count.
        /// </summary>
        void Reset();
    }
}
