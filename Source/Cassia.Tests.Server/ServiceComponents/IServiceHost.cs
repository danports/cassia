namespace Cassia.Tests.Server.ServiceComponents
{
    public interface IServiceHost
    {
        void Run(IHostedService service);
    }
}