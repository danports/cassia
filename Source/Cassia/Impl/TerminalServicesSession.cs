using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Security.Principal;

namespace Cassia.Impl
{
    /// <summary>
    /// Default implementation of <see cref="ITerminalServicesSession" />.
    /// </summary>
    public class TerminalServicesSession : ITerminalServicesSession
    {
        private readonly LazyLoadedProperty<int> _clientBuildNumber;
        private readonly LazyLoadedProperty<string> _clientDirectory;
        private readonly LazyLoadedProperty<IClientDisplay> _clientDisplay;
        private readonly LazyLoadedProperty<int> _clientHardwareId;
        private readonly LazyLoadedProperty<IPAddress> _clientIPAddress;
        private readonly string _clientName;
        private readonly LazyLoadedProperty<short> _clientProductId;
        private readonly LazyLoadedProperty<ClientProtocolType> _clientProtocolType;
        private readonly ConnectionState _connectionState;
        private readonly DateTime? _connectTime;
        private readonly DateTime? _currentTime;
        private readonly DateTime? _disconnectTime;
        private readonly string _domainName;
        private readonly DateTime? _lastInputTime;
        private readonly DateTime? _loginTime;
        private readonly ITerminalServer _server;
        private readonly int _sessionId;
        private readonly string _userName;
        private readonly string _windowStationName;

        public TerminalServicesSession(ITerminalServer server, int sessionId)
            : this(
                server, sessionId,
                NativeMethodsHelper.QuerySessionInformationForString(server.Handle, sessionId,
                                                                     WTS_INFO_CLASS.WTSWinStationName),
                NativeMethodsHelper.GetConnectionState(server.Handle, sessionId)) {}

        public TerminalServicesSession(ITerminalServer server, int sessionId, string windowStationName,
                                       ConnectionState connectionState)
        {
            _server = server;
            _sessionId = sessionId;
            _windowStationName = windowStationName;
            _connectionState = connectionState;
            _clientBuildNumber = new LazyLoadedProperty<int>(GetClientBuildNumber);
            _clientIPAddress = new LazyLoadedProperty<IPAddress>(GetClientIPAddress);
            _clientDisplay = new LazyLoadedProperty<IClientDisplay>(GetClientDisplay);
            _clientDirectory = new LazyLoadedProperty<string>(GetClientDirectory);
            _clientHardwareId = new LazyLoadedProperty<int>(GetClientHardwareId);
            _clientProductId = new LazyLoadedProperty<short>(GetClientProductId);
            _clientProtocolType = new LazyLoadedProperty<ClientProtocolType>(GetClientProtocolType);
            _clientName =
                NativeMethodsHelper.QuerySessionInformationForString(_server.Handle, _sessionId,
                                                                     WTS_INFO_CLASS.WTSClientName);

            // TODO: MSDN says most of these properties should be null for the console session.
            // I haven't observed this in practice on Windows Server 2000, 2003, or 2008, but perhaps this 
            // should be considered.
            if (Environment.OSVersion.Version >= new Version(6, 0))
            {
                // We can actually use documented APIs in Vista / Windows Server 2008+.
                WTSINFO info =
                    NativeMethodsHelper.QuerySessionInformationForStruct<WTSINFO>(server.Handle, _sessionId,
                                                                                  WTS_INFO_CLASS.WTSSessionInfo);
                _connectTime = NativeMethodsHelper.FileTimeToDateTime(info.ConnectTime);
                _currentTime = NativeMethodsHelper.FileTimeToDateTime(info.CurrentTime);
                _disconnectTime = NativeMethodsHelper.FileTimeToDateTime(info.DisconnectTime);
                _lastInputTime = NativeMethodsHelper.FileTimeToDateTime(info.LastInputTime);
                _loginTime = NativeMethodsHelper.FileTimeToDateTime(info.LogonTime);
                _userName = info.UserName;
                _domainName = info.Domain;
            }
            else
            {
                WINSTATIONINFORMATIONW wsInfo = NativeMethodsHelper.GetWinStationInformation(server.Handle, _sessionId);
                _connectTime = NativeMethodsHelper.FileTimeToDateTime(wsInfo.ConnectTime);
                _currentTime = NativeMethodsHelper.FileTimeToDateTime(wsInfo.CurrentTime);
                _disconnectTime = NativeMethodsHelper.FileTimeToDateTime(wsInfo.DisconnectTime);
                _lastInputTime = NativeMethodsHelper.FileTimeToDateTime(wsInfo.LastInputTime);
                _loginTime = NativeMethodsHelper.FileTimeToDateTime(wsInfo.LoginTime);
                _userName =
                    NativeMethodsHelper.QuerySessionInformationForString(server.Handle, _sessionId,
                                                                         WTS_INFO_CLASS.WTSUserName);
                _domainName =
                    NativeMethodsHelper.QuerySessionInformationForString(server.Handle, _sessionId,
                                                                         WTS_INFO_CLASS.WTSDomainName);
            }
        }

        public TerminalServicesSession(ITerminalServer server, WTS_SESSION_INFO sessionInfo)
            : this(server, sessionInfo.SessionID, sessionInfo.WinStationName, sessionInfo.State) {}

        #region ITerminalServicesSession Members

        public ClientProtocolType ClientProtocolType
        {
            get { return _clientProtocolType.Value; }
        }

        public short ClientProductId
        {
            get { return _clientProductId.Value; }
        }

        public int ClientHardwareId
        {
            get { return _clientHardwareId.Value; }
        }

        public string ClientDirectory
        {
            get { return _clientDirectory.Value; }
        }

        public IClientDisplay ClientDisplay
        {
            get { return _clientDisplay.Value; }
        }

        public int ClientBuildNumber
        {
            get { return _clientBuildNumber.Value; }
        }

        public ITerminalServer Server
        {
            get { return _server; }
        }

        public IPAddress ClientIPAddress
        {
            get { return _clientIPAddress.Value; }
        }

        public string WindowStationName
        {
            get { return _windowStationName; }
        }

        public string DomainName
        {
            get { return _domainName; }
        }

        public NTAccount UserAccount
        {
            get { return (string.IsNullOrEmpty(_userName) ? null : new NTAccount(_domainName, _userName)); }
        }

        public string ClientName
        {
            get { return _clientName; }
        }

        public ConnectionState ConnectionState
        {
            get { return _connectionState; }
        }

        public DateTime? ConnectTime
        {
            get { return _connectTime; }
        }

        public DateTime? CurrentTime
        {
            get { return _currentTime; }
        }

        public DateTime? DisconnectTime
        {
            get { return _disconnectTime; }
        }

        public DateTime? LastInputTime
        {
            get { return _lastInputTime; }
        }

        public DateTime? LoginTime
        {
            get { return _loginTime; }
        }

        public TimeSpan IdleTime
        {
            get
            {
                return
                    (_currentTime != null && _lastInputTime != null)
                        ? _currentTime.Value - _lastInputTime.Value
                        : TimeSpan.Zero;
            }
        }

        public int SessionId
        {
            get { return _sessionId; }
        }

        public string UserName
        {
            get { return _userName; }
        }

        public void Logoff()
        {
            Logoff(false);
        }

        public void Logoff(bool synchronous)
        {
            NativeMethodsHelper.LogoffSession(_server.Handle, _sessionId, synchronous);
        }

        public void Disconnect()
        {
            Disconnect(false);
        }

        public void Disconnect(bool synchronous)
        {
            NativeMethodsHelper.DisconnectSession(_server.Handle, _sessionId, synchronous);
        }

        public void MessageBox(string text)
        {
            MessageBox(text, null);
        }

        public void MessageBox(string text, string caption)
        {
            MessageBox(text, caption, default(RemoteMessageBoxIcon));
        }

        public void MessageBox(string text, string caption, RemoteMessageBoxIcon icon)
        {
            MessageBox(text, caption, default(RemoteMessageBoxButtons), icon, default(RemoteMessageBoxDefaultButton),
                       default(RemoteMessageBoxOptions), TimeSpan.Zero, false);
        }

        public RemoteMessageBoxResult MessageBox(string text, string caption, RemoteMessageBoxButtons buttons,
                                                 RemoteMessageBoxIcon icon, RemoteMessageBoxDefaultButton defaultButton,
                                                 RemoteMessageBoxOptions options, TimeSpan timeout, bool synchronous)
        {
            int timeoutSeconds = (int) timeout.TotalSeconds;
            int style = (int) buttons | (int) icon | (int) defaultButton | (int) options;
            // TODO: Win 2003 Server doesn't start timeout counter until user moves mouse in session.
            RemoteMessageBoxResult result =
                NativeMethodsHelper.SendMessage(_server.Handle, _sessionId, caption, text, style, timeoutSeconds,
                                                synchronous);
            // TODO: Windows Server 2008 R2 beta returns 0 if the timeout expires.
            // find out why this happens or file a bug report.
            return result == 0 ? RemoteMessageBoxResult.Timeout : result;
        }

        public IList<ITerminalServicesProcess> GetProcesses()
        {
            IList<ITerminalServicesProcess> allProcesses = _server.GetProcesses();
            List<ITerminalServicesProcess> results = new List<ITerminalServicesProcess>();
            foreach (ITerminalServicesProcess process in allProcesses)
            {
                if (process.SessionId == _sessionId)
                {
                    results.Add(process);
                }
            }
            return results;
        }

        #endregion

        private ClientProtocolType GetClientProtocolType()
        {
            return
                (ClientProtocolType)
                NativeMethodsHelper.QuerySessionInformationForShort(_server.Handle, _sessionId,
                                                                    WTS_INFO_CLASS.WTSClientProtocolType);
        }

        private short GetClientProductId()
        {
            return
                NativeMethodsHelper.QuerySessionInformationForShort(_server.Handle, _sessionId,
                                                                    WTS_INFO_CLASS.WTSClientProductId);
        }

        private int GetClientHardwareId()
        {
            return
                NativeMethodsHelper.QuerySessionInformationForInt(_server.Handle, _sessionId,
                                                                  WTS_INFO_CLASS.WTSClientHardwareId);
        }

        private string GetClientDirectory()
        {
            return
                NativeMethodsHelper.QuerySessionInformationForString(_server.Handle, _sessionId,
                                                                     WTS_INFO_CLASS.WTSClientDirectory);
        }

        private IClientDisplay GetClientDisplay()
        {
            WTS_CLIENT_DISPLAY clientDisplay =
                NativeMethodsHelper.QuerySessionInformationForStruct<WTS_CLIENT_DISPLAY>(_server.Handle, _sessionId,
                                                                                         WTS_INFO_CLASS.WTSClientDisplay);
            return new ClientDisplay(clientDisplay);
        }

        private IPAddress GetClientIPAddress()
        {
            WTS_CLIENT_ADDRESS clientAddress =
                NativeMethodsHelper.QuerySessionInformationForStruct<WTS_CLIENT_ADDRESS>(_server.Handle, _sessionId,
                                                                                         WTS_INFO_CLASS.WTSClientAddress);
            AddressFamily addressFamily = (AddressFamily) clientAddress.AddressFamily;
            if (addressFamily == AddressFamily.InterNetwork)
            {
                byte[] address = new byte[4];
                Array.Copy(clientAddress.Address, 2, address, 0, 4);
                return new IPAddress(address);
            }
            // TODO: support IPv6
            return null;
        }

        private int GetClientBuildNumber()
        {
            return
                NativeMethodsHelper.QuerySessionInformationForInt(_server.Handle, _sessionId,
                                                                  WTS_INFO_CLASS.WTSClientBuildNumber);
        }
    }
}