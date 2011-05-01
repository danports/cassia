using System;
using System.Runtime.InteropServices;
using System.ServiceModel;
using Cassia.Tests.Model;
using Cassia.Tests.Server.ServiceComponents;
using NetFwTypeLib;

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
            OpenServiceHost();
            OpenFirewallPort();
        }

        public void Stop()
        {
            CloseFirewallPort();
            CloseServiceHost();
        }

        public void Attach(IServiceHost host) {}

        #endregion

        private void OpenServiceHost()
        {
            _host = new ServiceHost(typeof(RemoteDesktopTestService));
            _host.AddServiceEndpoint(typeof(IRemoteDesktopTestService), new NetTcpBinding(),
                                     EndpointHelper.GetEndpointUri("localhost", EndpointHelper.DefaultPort));
            _host.Open();
        }

        private static void OpenFirewallPort()
        {
            try
            {
                var profile = GetCurrentFirewallProfile();
                if (!profile.FirewallEnabled)
                {
                    return;
                }
                var portType = Type.GetTypeFromProgID("HNetCfg.FWOpenPort", false);
                var port = (INetFwOpenPort) Activator.CreateInstance(portType);
                port.Name = "CassiaTestService";
                port.Port = EndpointHelper.DefaultPort;
                port.Protocol = NET_FW_IP_PROTOCOL_.NET_FW_IP_PROTOCOL_TCP;
                port.Scope = NET_FW_SCOPE_.NET_FW_SCOPE_ALL;
                port.IpVersion = NET_FW_IP_VERSION_.NET_FW_IP_VERSION_ANY;
                profile.GloballyOpenPorts.Add(port);
            }
            catch (COMException)
            {
                // Oh well, I guess we won't worry about the firewall.
            }
        }

        private static INetFwProfile GetCurrentFirewallProfile()
        {
            var type = Type.GetTypeFromProgID("HNetCfg.FwMgr", false);
            var manager = (INetFwMgr) Activator.CreateInstance(type);
            return manager.LocalPolicy.CurrentProfile;
        }

        private static void CloseFirewallPort()
        {
            try
            {
                var profile = GetCurrentFirewallProfile();
                if (!profile.FirewallEnabled)
                {
                    return;
                }
                profile.GloballyOpenPorts.Remove(EndpointHelper.DefaultPort, NET_FW_IP_PROTOCOL_.NET_FW_IP_PROTOCOL_TCP);
            }
            catch (ArgumentException) {}
            catch (COMException) {}
        }

        private void CloseServiceHost()
        {
            if (_host == null)
            {
                return;
            }
            _host.Close();
            _host = null;
        }
    }
}