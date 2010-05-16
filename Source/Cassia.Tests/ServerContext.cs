using System;
using System.ServiceModel;
using System.ServiceProcess;
using Cassia.Tests.Model;

namespace Cassia.Tests
{
    public class ServerContext : IDisposable
    {
        private readonly string _server;
        private ChannelFactory<IRemoteDesktopTestService> _channelFactory;
        private ServiceController _serviceController;
        private IRemoteDesktopTestService _testService;

        public ServerContext(string server)
        {
            _server = server;
        }

        public IRemoteDesktopTestService TestService
        {
            get
            {
                if (_testService == null)
                {
                    _serviceController = new ServiceController("CassiaTestServer", _server);
                    _serviceController.Start();

                    NetTcpBinding binding = new NetTcpBinding();
                    binding.Security.Mode = SecurityMode.None;
                    string remoteAddress = String.Format("net.tcp://{0}:17876/CassiaTestService", _server);
                    _channelFactory = new ChannelFactory<IRemoteDesktopTestService>(binding, remoteAddress);
                    _testService = _channelFactory.CreateChannel();
                }
                return _testService;
            }
        }

        public string ServerName
        {
            get { return _server; }
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (_channelFactory != null)
            {
                _channelFactory.Close();
            }
            if (_serviceController != null)
            {
                _serviceController.Stop();
                _serviceController.Dispose();
            }
        }

        #endregion

        public RdpConnection OpenRdpConnection()
        {
            return new RdpConnection(this);
        }
    }
}