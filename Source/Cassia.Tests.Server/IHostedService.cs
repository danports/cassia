namespace Cassia.Tests.Server
{
    public interface IHostedService
    {
        string Name { get; }
        void Start();
        void Stop();
    }
}