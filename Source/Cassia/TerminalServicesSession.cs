using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Security.Principal;

namespace Cassia
{
    /// <summary>
    /// Default implementation of <see cref="ITerminalServicesSession" />.
    /// </summary>
    public class TerminalServicesSession : ITerminalServicesSession
    {
        private readonly string _clientName;
        private readonly WTS_CONNECTSTATE_CLASS _connectionState;
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
        private int _bitsPerPixel;
        private int _clientBuildNumber;
        private bool _fetchedClientBuildNumber;
        private bool _fetchedClientDisplay;
        private bool _fetchedIpAddress;
        private int _horizontalResolution;
        private IPAddress _ipAddress;
        private int _verticalResolution;

        public TerminalServicesSession(ITerminalServer server, int sessionId)
        {
            _server = server;
            _sessionId = sessionId;
            _clientName =
                NativeMethodsHelper.QuerySessionInformationForString(_server.Handle, _sessionId,
                                                                     WTS_INFO_CLASS.WTSClientName);

            // TODO: more lazy loading here.
            // TODO: MSDN says most of these properties should be null for the console session.
            // I haven't observed this in practice on Windows Server 2000, 2003, or 2008, but perhaps this 
            // should be considered.
            if (Environment.OSVersion.Version > new Version(6, 0))
            {
                // We can actually use documented APIs in Vista / Windows Server 2008+.
                WTSINFO info =
                    NativeMethodsHelper.QuerySessionInformationForStruct<WTSINFO>(server.Handle, sessionId,
                                                                                  WTS_INFO_CLASS.WTSSessionInfo);
                _connectTime = DateTime.FromFileTime(info.ConnectTime);
                _currentTime = DateTime.FromFileTime(info.CurrentTime);
                _disconnectTime = DateTime.FromFileTime(info.DisconnectTime);
                _lastInputTime = DateTime.FromFileTime(info.LastInputTime);
                _loginTime = DateTime.FromFileTime(info.LogonTime);
                _userName = info.UserName;
                _domainName = info.Domain;
                _connectionState = info.State;
                _windowStationName = info.WinStationName;
            }
            else
            {
                WINSTATIONINFORMATIONW wsInfo = NativeMethodsHelper.GetWinStationInformation(server.Handle, sessionId);
                _connectTime = NativeMethodsHelper.FileTimeToDateTime(wsInfo.ConnectTime);
                _currentTime = NativeMethodsHelper.FileTimeToDateTime(wsInfo.CurrentTime);
                _disconnectTime = NativeMethodsHelper.FileTimeToDateTime(wsInfo.DisconnectTime);
                _lastInputTime = NativeMethodsHelper.FileTimeToDateTime(wsInfo.LastInputTime);
                _loginTime = NativeMethodsHelper.FileTimeToDateTime(wsInfo.LoginTime);
                _connectionState = NativeMethodsHelper.GetConnectionState(server.Handle, sessionId);
                _userName =
                    NativeMethodsHelper.QuerySessionInformationForString(server.Handle, sessionId,
                                                                         WTS_INFO_CLASS.WTSUserName);
                _domainName =
                    NativeMethodsHelper.QuerySessionInformationForString(server.Handle, sessionId,
                                                                         WTS_INFO_CLASS.WTSDomainName);
                _windowStationName =
                    NativeMethodsHelper.QuerySessionInformationForString(server.Handle, sessionId,
                                                                         WTS_INFO_CLASS.WTSWinStationName);
            }
        }

        #region ITerminalServicesSession Members

        public int ClientBuildNumber
        {
            get
            {
                CheckClientBuildNumber();
                return _clientBuildNumber;
            }
        }

        public ITerminalServer Server
        {
            get { return _server; }
        }

        public IPAddress ClientIPAddress
        {
            get
            {
                if (!_fetchedIpAddress)
                {
                    WTS_CLIENT_ADDRESS clientAddress =
                        NativeMethodsHelper.QuerySessionInformationForStruct<WTS_CLIENT_ADDRESS>(_server.Handle,
                                                                                                 _sessionId,
                                                                                                 WTS_INFO_CLASS.
                                                                                                     WTSClientAddress);
                    AddressFamily addressFamily = (AddressFamily) clientAddress.AddressFamily;
                    if (addressFamily == AddressFamily.InterNetwork)
                    {
                        byte[] address = new byte[4];
                        Array.Copy(clientAddress.Address, 2, address, 0, 4);
                        _ipAddress = new IPAddress(address);
                    }
                    _fetchedIpAddress = true;
                }
                return _ipAddress;
            }
        }

        public string WindowStationName
        {
            get { return _windowStationName; }
        }

        public int BitsPerPixel
        {
            get
            {
                CheckClientDisplay();
                return _bitsPerPixel;
            }
        }

        public int HorizontalResolution
        {
            get
            {
                CheckClientDisplay();
                return _horizontalResolution;
            }
        }

        public int VerticalResolution
        {
            get
            {
                CheckClientDisplay();
                return _verticalResolution;
            }
        }

        public string DomainName
        {
            get { return _domainName; }
        }

        public NTAccount Account
        {
            get { return (string.IsNullOrEmpty(_userName) ? null : new NTAccount(_domainName, _userName)); }
        }

        public string ClientName
        {
            get { return _clientName; }
        }

        public WTS_CONNECTSTATE_CLASS ConnectionState
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
            return
                NativeMethodsHelper.SendMessage(_server.Handle, _sessionId, caption, text, style, timeoutSeconds,
                                                synchronous);
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

        private void CheckClientBuildNumber()
        {
            if (_fetchedClientBuildNumber)
            {
                return;
            }
            _clientBuildNumber =
                NativeMethodsHelper.QuerySessionInformationForClientBuildNumber(_server.Handle, _sessionId);
            _fetchedClientBuildNumber = true;
        }

        private void CheckClientDisplay()
        {
            if (_fetchedClientDisplay)
            {
                return;
            }
            WTS_CLIENT_DISPLAY clientDisplay =
                NativeMethodsHelper.QuerySessionInformationForStruct<WTS_CLIENT_DISPLAY>(_server.Handle, _sessionId,
                                                                                         WTS_INFO_CLASS.WTSClientDisplay);
            _horizontalResolution = clientDisplay.HorizontalResolution;
            _verticalResolution = clientDisplay.VerticalResolution;
            _bitsPerPixel = GetBitsPerPixel(clientDisplay.ColorDepth);
            _fetchedClientDisplay = true;
        }

        private static int GetBitsPerPixel(int colorDepth)
        {
            switch (colorDepth)
            {
                case 1:
                    return 4;
                case 2:
                    return 8;
                case 4:
                    return 16;
                case 8:
                    return 24;
                case 16:
                    return 15;
            }
            return 0;
        }
    }
}