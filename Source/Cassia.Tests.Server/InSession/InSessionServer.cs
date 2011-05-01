using System.ServiceModel;
using System.Threading;

namespace Cassia.Tests.Server.InSession
{
    public static class InSessionServer
    {
        public const string EndpointUri = "net.tcp://localhost:8973/CassiaInSessionService";
        private static ManualResetEvent _stopEvent;

        public static void Run()
        {
            _stopEvent = new ManualResetEvent(false);
            var host = new ServiceHost(typeof(InSessionTestService));
            host.AddServiceEndpoint(typeof(IInSessionTestService), new NetTcpBinding(), EndpointUri);
            host.Open();
            _stopEvent.WaitOne();
            Thread.Sleep(500); // let any pending calls complete...
            Logger.InSessionLog("Stopping the WCF host");
            host.Close();
        }

        public static void Stop()
        {
            _stopEvent.Set();
        }
    }
}