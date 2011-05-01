namespace Cassia.Tests.Server.ServiceComponents
{
    public interface IHostedService
    {
        string Name { get; }
        void Start();
        void Stop();
        void Attach(IServiceHost host);
    }
}