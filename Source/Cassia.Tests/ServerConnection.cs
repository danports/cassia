using System;
using System.ComponentModel;
using System.IO;
using System.ServiceModel;
using System.ServiceProcess;
using System.Threading;
using Cassia.Tests.Model;

namespace Cassia.Tests
{
    public class ServerConnection : IDisposable
    {
        private readonly ServerInfo _server;
        private ChannelFactory<IRemoteDesktopTestService> _channelFactory;
        private ServiceController _serviceController;
        private IRemoteDesktopTestService _testService;

        public ServerConnection(ServerInfo server)
        {
            _server = server;
        }

        public IRemoteDesktopTestService TestService
        {
            get
            {
                if (_testService == null)
                {
                    CopyFilesToServer();
                    CreateAndStartService();
                    ConnectToService();
                }
                return _testService;
            }
        }

        public ServerInfo Server
        {
            get { return _server; }
        }

        private string TargetDirectory
        {
            get { return string.Format(@"\\{0}\ADMIN$\Temp\CassiaTestServer", _server); }
        }

        #region IDisposable Members

        public void Dispose()
        {
            DisconnectFromService();
            StopAndDeleteService();
            DeleteFilesFromServer();
        }

        #endregion

        private void DeleteFilesFromServer()
        {
            if (!Directory.Exists(TargetDirectory))
            {
                return;
            }
            Directory.Delete(TargetDirectory, true);
        }

        private void StopAndDeleteService()
        {
            if (_serviceController == null)
            {
                return;
            }
            try
            {
                _serviceController.Stop();
                _serviceController.WaitForStatus(ServiceControllerStatus.Stopped);
                _serviceController.Dispose();
            }
            catch (Win32Exception) {}
            // It takes Windows a bit of time after the service stops to release locks on the assemblies, apparently.
            Thread.Sleep(500);
            ServiceHelper.Delete(_serviceController);
        }

        private void DisconnectFromService()
        {
            if (_channelFactory == null)
            {
                return;
            }
            try
            {
                _channelFactory.Close();
            }
            catch (CommunicationObjectFaultedException) {}
        }

        private void CopyFilesToServer()
        {
            var targetDirectory = TargetDirectory;
            if (!Directory.Exists(targetDirectory))
            {
                Directory.CreateDirectory(targetDirectory);
            }
            const string sourceDirectory = @"..\..\..\Cassia.Tests.Server\bin\Debug";
            foreach (FileInfo fileInfo in new DirectoryInfo(sourceDirectory).GetFiles())
            {
                fileInfo.CopyTo(Path.Combine(targetDirectory, fileInfo.Name), true);
            }
        }

        private void ConnectToService()
        {
            var binding = new NetTcpBinding();
            var remoteAddress = EndpointHelper.GetEndpointUri(_server.Name, EndpointHelper.DefaultPort);
            _channelFactory = new ChannelFactory<IRemoteDesktopTestService>(binding, remoteAddress);
            _testService = _channelFactory.CreateChannel();
        }

        private void CreateAndStartService()
        {
            _serviceController = ServiceHelper.Create(_server.Name, "CassiaTestServer", "Cassia Test Server",
                                                      ServiceType.Win32OwnProcess, ServiceStartMode.Automatic,
                                                      ServiceHelper.ServiceErrorControl.Normal,
                                                      @"C:\Windows\Temp\CassiaTestServer\Cassia.Tests.Server.exe", null,
                                                      new[] {"TermService"}, null, null);
            _serviceController.Start();
            _serviceController.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromMinutes(30));
        }

        public RdpConnection OpenRdpConnection()
        {
            return new RdpConnection(this);
        }
    }
}