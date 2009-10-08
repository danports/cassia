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
        private readonly LazyLoadedProperty<string> _applicationName;
        private readonly LazyLoadedProperty<int> _clientBuildNumber;
        private readonly LazyLoadedProperty<string> _clientDirectory;
        private readonly LazyLoadedProperty<IClientDisplay> _clientDisplay;
        private readonly LazyLoadedProperty<int> _clientHardwareId;
        private readonly LazyLoadedProperty<IPAddress> _clientIPAddress;
        private readonly LazyLoadedProperty<string> _clientName;
        private readonly LazyLoadedProperty<short> _clientProductId;
        private readonly LazyLoadedProperty<ClientProtocolType> _clientProtocolType;
        private readonly LazyLoadedProperty<ConnectionState> _connectionState;
        private readonly GroupLazyLoadedProperty<DateTime?> _connectTime;
        private readonly GroupLazyLoadedProperty<DateTime?> _currentTime;
        private readonly GroupLazyLoadedProperty<DateTime?> _disconnectTime;
        private readonly GroupLazyLoadedProperty<string> _domainName;
        private readonly LazyLoadedProperty<string> _initialProgram;
        private readonly GroupLazyLoadedProperty<DateTime?> _lastInputTime;
        private readonly GroupLazyLoadedProperty<DateTime?> _loginTime;
        private readonly LazyLoadedProperty<EndPoint> _remoteEndPoint;
        private readonly ITerminalServer _server;
        private readonly int _sessionId;
        private readonly GroupLazyLoadedProperty<string> _userName;
        private readonly LazyLoadedProperty<string> _windowStationName;
        private readonly LazyLoadedProperty<string> _workingDirectory;

        public TerminalServicesSession(ITerminalServer server, int sessionId)
        {
            _server = server;
            _sessionId = sessionId;

            // TODO: on Windows Server 2008, most of these values can be fetched in one shot from WTSCLIENT.
            // Do this with GroupLazyLoadedProperty.
            _windowStationName = new LazyLoadedProperty<string>(GetWindowStationName);
            _connectionState = new LazyLoadedProperty<ConnectionState>(GetConnectionState);
            _clientBuildNumber = new LazyLoadedProperty<int>(GetClientBuildNumber);
            _clientIPAddress = new LazyLoadedProperty<IPAddress>(GetClientIPAddress);
            _remoteEndPoint = new LazyLoadedProperty<EndPoint>(GetRemoteEndPoint);
            _clientDisplay = new LazyLoadedProperty<IClientDisplay>(GetClientDisplay);
            _clientDirectory = new LazyLoadedProperty<string>(GetClientDirectory);
            _workingDirectory = new LazyLoadedProperty<string>(GetWorkingDirectory);
            _initialProgram = new LazyLoadedProperty<string>(GetInitialProgram);
            _applicationName = new LazyLoadedProperty<string>(GetApplicationName);
            _clientHardwareId = new LazyLoadedProperty<int>(GetClientHardwareId);
            _clientProductId = new LazyLoadedProperty<short>(GetClientProductId);
            _clientProtocolType = new LazyLoadedProperty<ClientProtocolType>(GetClientProtocolType);
            _clientName = new LazyLoadedProperty<string>(GetClientName);

            // TODO: MSDN says most of these properties should be null for the console session.
            // I haven't observed this in practice on Windows Server 2000, 2003, or 2008, but perhaps this 
            // should be considered.
            GroupPropertyLoader loader = IsVistaSp1OrHigher
                                             ? (GroupPropertyLoader) LoadWtsInfoProperties
                                             : LoadWinStationInformationProperties;
            _connectTime = new GroupLazyLoadedProperty<DateTime?>(loader);
            _currentTime = new GroupLazyLoadedProperty<DateTime?>(loader);
            _disconnectTime = new GroupLazyLoadedProperty<DateTime?>(loader);
            _lastInputTime = new GroupLazyLoadedProperty<DateTime?>(loader);
            _loginTime = new GroupLazyLoadedProperty<DateTime?>(loader);
            _userName = new GroupLazyLoadedProperty<string>(loader);
            _domainName = new GroupLazyLoadedProperty<string>(loader);
        }

        public TerminalServicesSession(ITerminalServer server, WTS_SESSION_INFO sessionInfo)
            : this(server, sessionInfo.SessionID)
        {
            _windowStationName.Value = sessionInfo.WinStationName;
            _connectionState.Value = sessionInfo.State;
        }

        private static bool IsVistaSp1OrHigher
        {
            get { return Environment.OSVersion.Version >= new Version(6, 0, 6001); }
        }

        #region ITerminalServicesSession Members

        public string ApplicationName
        {
            get { return _applicationName.Value; }
        }

        public EndPoint RemoteEndPoint
        {
            get { return _remoteEndPoint.Value; }
        }

        public string InitialProgram
        {
            get { return _initialProgram.Value; }
        }

        public string WorkingDirectory
        {
            get { return _workingDirectory.Value; }
        }

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
            get { return _windowStationName.Value; }
        }

        public string DomainName
        {
            get { return _domainName.Value; }
        }

        public NTAccount UserAccount
        {
            get { return (string.IsNullOrEmpty(UserName) ? null : new NTAccount(DomainName, UserName)); }
        }

        public string ClientName
        {
            get { return _clientName.Value; }
        }

        public ConnectionState ConnectionState
        {
            get { return _connectionState.Value; }
        }

        public DateTime? ConnectTime
        {
            get { return _connectTime.Value; }
        }

        public DateTime? CurrentTime
        {
            get { return _currentTime.Value; }
        }

        public DateTime? DisconnectTime
        {
            get { return _disconnectTime.Value; }
        }

        public DateTime? LastInputTime
        {
            get { return _lastInputTime.Value; }
        }

        public DateTime? LoginTime
        {
            get { return _loginTime.Value; }
        }

        public TimeSpan IdleTime
        {
            get
            {
                if (ConnectionState == ConnectionState.Disconnected)
                {
                    if (CurrentTime != null && DisconnectTime != null)
                    {
                        return CurrentTime.Value - DisconnectTime.Value;
                    }
                }
                else
                {
                    if (CurrentTime != null && LastInputTime != null)
                    {
                        return CurrentTime.Value - LastInputTime.Value;
                    }
                }
                return TimeSpan.Zero;
            }
        }

        public int SessionId
        {
            get { return _sessionId; }
        }

        public string UserName
        {
            get { return _userName.Value; }
        }

        public void Logoff()
        {
            Logoff(true);
        }

        public void Logoff(bool synchronous)
        {
            NativeMethodsHelper.LogoffSession(_server.Handle, _sessionId, synchronous);
        }

        public void Disconnect()
        {
            Disconnect(true);
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
            RemoteMessageBoxResult result = NativeMethodsHelper.SendMessage(_server.Handle, _sessionId, caption, text,
                                                                            style, timeoutSeconds, synchronous);
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

        public void StartRemoteControl(ConsoleKey hotkey, RemoteControlHotkeyModifiers hotkeyModifiers)
        {
            if (IsVistaSp1OrHigher)
            {
                NativeMethodsHelper.StartRemoteControl(_server.Handle, _sessionId, hotkey, hotkeyModifiers);
            }
            else
            {
                NativeMethodsHelper.LegacyStartRemoteControl(_server.Handle, _sessionId, hotkey, hotkeyModifiers);
            }
        }

        #endregion

        private ConnectionState GetConnectionState()
        {
            return NativeMethodsHelper.GetConnectionState(_server.Handle, _sessionId);
        }

        private string GetWindowStationName()
        {
            return NativeMethodsHelper.QuerySessionInformationForString(_server.Handle, _sessionId,
                                                                        WTS_INFO_CLASS.WTSWinStationName);
        }

        private void LoadWinStationInformationProperties()
        {
            WINSTATIONINFORMATIONW wsInfo = NativeMethodsHelper.GetWinStationInformation(_server.Handle, _sessionId);
            _connectTime.Value = NativeMethodsHelper.FileTimeToDateTime(wsInfo.ConnectTime);
            _currentTime.Value = NativeMethodsHelper.FileTimeToDateTime(wsInfo.CurrentTime);
            _disconnectTime.Value = NativeMethodsHelper.FileTimeToDateTime(wsInfo.DisconnectTime);
            _lastInputTime.Value = NativeMethodsHelper.FileTimeToDateTime(wsInfo.LastInputTime);
            _loginTime.Value = NativeMethodsHelper.FileTimeToDateTime(wsInfo.LoginTime);
            _userName.Value = wsInfo.UserName;
            _domainName.Value = wsInfo.Domain;
        }

        private void LoadWtsInfoProperties()
        {
            WTSINFO info = NativeMethodsHelper.QuerySessionInformationForStruct<WTSINFO>(_server.Handle, _sessionId,
                                                                                         WTS_INFO_CLASS.WTSSessionInfo);
            _connectTime.Value = NativeMethodsHelper.FileTimeToDateTime(info.ConnectTime);
            _currentTime.Value = NativeMethodsHelper.FileTimeToDateTime(info.CurrentTime);
            _disconnectTime.Value = NativeMethodsHelper.FileTimeToDateTime(info.DisconnectTime);
            _lastInputTime.Value = NativeMethodsHelper.FileTimeToDateTime(info.LastInputTime);
            _loginTime.Value = NativeMethodsHelper.FileTimeToDateTime(info.LogonTime);
            _userName.Value = info.UserName;
            _domainName.Value = info.Domain;
        }

        private string GetClientName()
        {
            return NativeMethodsHelper.QuerySessionInformationForString(_server.Handle, _sessionId,
                                                                        WTS_INFO_CLASS.WTSClientName);
        }

        private string GetApplicationName()
        {
            return NativeMethodsHelper.QuerySessionInformationForString(_server.Handle, _sessionId,
                                                                        WTS_INFO_CLASS.WTSApplicationName);
        }

        private EndPoint GetRemoteEndPoint()
        {
            return NativeMethodsHelper.QuerySessionInformationForEndPoint(_server.Handle, _sessionId);
        }

        private string GetInitialProgram()
        {
            return NativeMethodsHelper.QuerySessionInformationForString(_server.Handle, _sessionId,
                                                                        WTS_INFO_CLASS.WTSInitialProgram);
        }

        private string GetWorkingDirectory()
        {
            return NativeMethodsHelper.QuerySessionInformationForString(_server.Handle, _sessionId,
                                                                        WTS_INFO_CLASS.WTSWorkingDirectory);
        }

        private ClientProtocolType GetClientProtocolType()
        {
            return
                (ClientProtocolType)
                NativeMethodsHelper.QuerySessionInformationForShort(_server.Handle, _sessionId,
                                                                    WTS_INFO_CLASS.WTSClientProtocolType);
        }

        private short GetClientProductId()
        {
            return NativeMethodsHelper.QuerySessionInformationForShort(_server.Handle, _sessionId,
                                                                       WTS_INFO_CLASS.WTSClientProductId);
        }

        private int GetClientHardwareId()
        {
            return NativeMethodsHelper.QuerySessionInformationForInt(_server.Handle, _sessionId,
                                                                     WTS_INFO_CLASS.WTSClientHardwareId);
        }

        private string GetClientDirectory()
        {
            return NativeMethodsHelper.QuerySessionInformationForString(_server.Handle, _sessionId,
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
            return NativeMethodsHelper.QuerySessionInformationForInt(_server.Handle, _sessionId,
                                                                     WTS_INFO_CLASS.WTSClientBuildNumber);
        }
    }
}