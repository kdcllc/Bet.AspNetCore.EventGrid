using System.Threading;

namespace Bet.AspNetCore.EventGrid.Diagnostics
{
    public class ProcessReport : IProcessReport
    {
        private int _succeeded;
        private int _failed;

        /// <inheritdoc/>
        public int SuccessCount => _succeeded;

        /// <inheritdoc/>
        public int FailureCount => _failed;

        /// <inheritdoc/>
        public int IncrementSuccess()
        {
            return Interlocked.Increment(ref _succeeded);
        }

        /// <inheritdoc/>
        public int IncrementFailure()
        {
            return Interlocked.Increment(ref _failed);
        }

        /// <inheritdoc/>
        public void Reset()
        {
            Interlocked.Exchange(ref _succeeded, 0);
            Interlocked.Exchange(ref _failed, 0);
        }
    }
}
