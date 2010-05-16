namespace Cassia.Tests.Server
{
    public interface IServiceHost
    {
        void Run(IHostedService service);
    }
}