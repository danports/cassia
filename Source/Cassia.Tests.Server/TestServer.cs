using System.ServiceModel;
using Cassia.Tests.Model;

namespace Cassia.Tests.Server
{
    public class TestServer : IHostedService
    {
        private ServiceHost _host;

        #region IHostedService Members

        public string Name
        {
            get { return "CassiaTestService"; }
        }

        public void Start()
        {
            _host = new ServiceHost(typeof(RemoteDesktopTestService));
            NetTcpBinding binding = new NetTcpBinding();
            binding.Security.Mode = SecurityMode.None;
            _host.AddServiceEndpoint(typeof(IRemoteDesktopTestService), binding,
                                     "net.tcp://localhost:17876/CassiaTestService");
            _host.Open();
        }

        public void Stop()
        {
            if (_host == null)
            {
                return;
            }
            _host.Close();
            _host = null;
        }

        #endregion
    }
}