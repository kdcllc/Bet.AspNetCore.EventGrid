namespace Bet.AspNetCore.EventGrid.WebApp.Services
{
    public interface IOperationService
    {
        IOperationScoped ScopedOperation { get; }

        IOperationSingletonInstance SingletonInstanceOperation { get; }

        IOperationSingleton SingletonOperation { get; }

        IOperationTransient TransientOperation { get; }
    }
}