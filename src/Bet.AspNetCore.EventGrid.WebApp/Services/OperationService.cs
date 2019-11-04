namespace Bet.AspNetCore.EventGrid.WebApp.Services
{
    public class OperationService : IOperationService
    {
        public OperationService(
            IOperationTransient transientOperation,
            IOperationScoped scopedOperation,
            IOperationSingleton singletonOperation,
            IOperationSingletonInstance instanceOperation)
        {
            TransientOperation = transientOperation;
            ScopedOperation = scopedOperation;
            SingletonOperation = singletonOperation;
            SingletonInstanceOperation = instanceOperation;
        }

        public IOperationTransient TransientOperation { get; }

        public IOperationScoped ScopedOperation { get; }

        public IOperationSingleton SingletonOperation { get; }

        public IOperationSingletonInstance SingletonInstanceOperation { get; }
    }
}
