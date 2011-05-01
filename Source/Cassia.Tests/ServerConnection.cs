using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.ServiceModel;
using System.ServiceProcess;
using System.Threading;
using Cassia.Tests.Model;

namespace Cassia.Tests
{
    public class ServerConnection : IDisposable
    {
        private const string _serviceName = "CassiaTestServer";
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
                    CreateService();
                    StartService();
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
            StopService();
            DeleteService();
            DeleteFilesFromServer();
        }

        #endregion

        private void StartService()
        {
            _serviceController.Start();
            _serviceController.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromMinutes(30));
        }

        private void DeleteService()
        {
            ServiceHelper.DeleteIfExists(_server.Name, _serviceName);
        }

        private void DeleteFilesFromServer()
        {
            if (!Directory.Exists(TargetDirectory))
            {
                return;
            }
            try
            {
                Directory.Delete(TargetDirectory, true);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("wasn't able to delete files from server: " + ex);
            }
        }

        private void StopService()
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
            catch (InvalidOperationException) {}
            catch (Win32Exception) {}
            // It takes Windows a bit of time after the service stops to release locks on the assemblies, apparently.
            Thread.Sleep(500);
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

        private void CreateService()
        {
            _serviceController = ServiceHelper.Create(_server.Name, _serviceName, "Cassia Test Server",
                                                      ServiceType.Win32OwnProcess, ServiceStartMode.Automatic,
                                                      ServiceHelper.ServiceErrorControl.Normal,
                                                      // TODO: this path is not DRY...
                                                      @"C:\Windows\Temp\CassiaTestServer\Cassia.Tests.Server.exe", null,
                                                      new[] {"TermService"}, null, null);
        }

        public RdpConnection OpenRdpConnection()
        {
            return new RdpConnection(this);
        }
    }
}