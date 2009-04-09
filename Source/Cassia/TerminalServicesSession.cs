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
        private readonly WTS_CONNECTSTATE_CLASS _connectionState;
        private readonly DateTime _connectTime;
        private readonly DateTime _currentTime;
        private readonly DateTime _disconnectTime;
        private readonly string _domainName;
        private readonly DateTime _lastInputTime;
        private readonly DateTime _loginTime;
        private readonly ITerminalServer _server;
        private readonly int _sessionId;
        private readonly string _userName;
        private readonly string _windowStationName;
        private int _bitsPerPixel;
        private string _clientName;
        private bool _fetchedClientDisplay;
        private bool _fetchedClientName;
        private bool _fetchedIpAddress;
        private int _horizontalResolution;
        private IPAddress _ipAddress;
        private int _verticalResolution;

        public TerminalServicesSession(ITerminalServer server, int sessionId)
        {
            _server = server;
            _sessionId = sessionId;

            // TODO: lazy loading.
            if (Environment.OSVersion.Version > new Version(6, 0))
            {
                // We can actually use documented APIs in Vista / Windows Server 2008+.
                WTSINFO info =
                    SessionHelper.QuerySessionInformationForStruct<WTSINFO>(server.Handle, sessionId,
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
                WINSTATIONINFORMATIONW wsInfo = SessionHelper.GetWinStationInformation(server.Handle, sessionId);
                _connectTime = SessionHelper.FileTimeToDateTime(wsInfo.ConnectTime);
                _currentTime = SessionHelper.FileTimeToDateTime(wsInfo.CurrentTime);
                _disconnectTime = SessionHelper.FileTimeToDateTime(wsInfo.DisconnectTime);
                _lastInputTime = SessionHelper.FileTimeToDateTime(wsInfo.LastInputTime);
                _loginTime = SessionHelper.FileTimeToDateTime(wsInfo.LoginTime);
                _connectionState = SessionHelper.GetConnectionState(server.Handle, sessionId);
                _userName =
                    SessionHelper.QuerySessionInformationForString(server.Handle, sessionId, WTS_INFO_CLASS.WTSUserName);
                _domainName =
                    SessionHelper.QuerySessionInformationForString(server.Handle, sessionId,
                                                                   WTS_INFO_CLASS.WTSDomainName);
                _windowStationName =
                    SessionHelper.QuerySessionInformationForString(server.Handle, sessionId,
                                                                   WTS_INFO_CLASS.WTSWinStationName);
            }
        }

        #region ITerminalServicesSession Members

        public IPAddress ClientIPAddress
        {
            get
            {
                if (!_fetchedIpAddress)
                {
                    WTS_CLIENT_ADDRESS clientAddress =
                        SessionHelper.QuerySessionInformationForStruct<WTS_CLIENT_ADDRESS>(_server.Handle, _sessionId,
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
            get
            {
                CheckClientName();
                return _clientName;
            }
        }

        public WTS_CONNECTSTATE_CLASS ConnectionState
        {
            get { return _connectionState; }
        }

        public DateTime ConnectTime
        {
            get { return _connectTime; }
        }

        public DateTime CurrentTime
        {
            get { return _currentTime; }
        }

        public DateTime DisconnectTime
        {
            get { return _disconnectTime; }
        }

        public DateTime LastInputTime
        {
            get { return _lastInputTime; }
        }

        public DateTime LoginTime
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
            SessionHelper.LogoffSession(_server.Handle, _sessionId, synchronous);
        }

        public void Disconnect()
        {
            Disconnect(false);
        }

        public void Disconnect(bool synchronous)
        {
            SessionHelper.DisconnectSession(_server.Handle, _sessionId, synchronous);
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
                SessionHelper.SendMessage(_server.Handle, _sessionId, caption, text, style, timeoutSeconds, synchronous);
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

        private void CheckClientName()
        {
            if (_fetchedClientName)
            {
                return;
            }
            _clientName =
                SessionHelper.QuerySessionInformationForString(_server.Handle, _sessionId, WTS_INFO_CLASS.WTSClientName);
            _fetchedClientName = true;
        }

        private void CheckClientDisplay()
        {
            if (_fetchedClientDisplay)
            {
                return;
            }
            WTS_CLIENT_DISPLAY clientDisplay =
                SessionHelper.QuerySessionInformationForStruct<WTS_CLIENT_DISPLAY>(_server.Handle, _sessionId,
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