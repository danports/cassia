using System;
using System.ServiceModel;
using Cassia.Tests.Model;
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

        #endregion

        private void OpenServiceHost()
        {
            _host = new ServiceHost(typeof(RemoteDesktopTestService));
            NetTcpBinding binding = new NetTcpBinding();
            binding.Security.Mode = SecurityMode.None;
            _host.AddServiceEndpoint(typeof(IRemoteDesktopTestService), binding,
                                     EndpointHelper.GetEndpointUri("localhost", EndpointHelper.DefaultPort));
            _host.Open();
        }

        private static void OpenFirewallPort()
        {
            INetFwProfile profile = GetCurrentFirewallProfile();
            if (!profile.FirewallEnabled)
            {
                return;
            }
            Type portType = Type.GetTypeFromProgID("HNetCfg.FWOpenPort", false);
            INetFwOpenPort port = (INetFwOpenPort) Activator.CreateInstance(portType);
            port.Name = "CassiaTestService";
            port.Port = EndpointHelper.DefaultPort;
            port.Protocol = NET_FW_IP_PROTOCOL_.NET_FW_IP_PROTOCOL_TCP;
            port.Scope = NET_FW_SCOPE_.NET_FW_SCOPE_ALL;
            port.IpVersion = NET_FW_IP_VERSION_.NET_FW_IP_VERSION_ANY;
            profile.GloballyOpenPorts.Add(port);
        }

        private static INetFwProfile GetCurrentFirewallProfile()
        {
            Type type = Type.GetTypeFromProgID("HNetCfg.FwMgr", false);
            INetFwMgr manager = (INetFwMgr) Activator.CreateInstance(type);
            return manager.LocalPolicy.CurrentProfile;
        }

        private static void CloseFirewallPort()
        {
            try
            {
                INetFwProfile profile = GetCurrentFirewallProfile();
                if (!profile.FirewallEnabled)
                {
                    return;
                }
                profile.GloballyOpenPorts.Remove(EndpointHelper.DefaultPort, NET_FW_IP_PROTOCOL_.NET_FW_IP_PROTOCOL_TCP);
            }
            catch (ArgumentException) {}
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