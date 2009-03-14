using System;
using System.Collections.Generic;

namespace Cassia
{
    public class TerminalServicesSession : ITerminalServicesSession
    {
        private readonly string _clientName;
        private readonly WTS_CONNECTSTATE_CLASS _connectionState;
        private readonly DateTime _connectTime;
        private readonly DateTime _currentTime;
        private readonly DateTime _disconnectTime;
        private readonly DateTime _lastInputTime;
        private readonly DateTime _loginTime;
        private readonly ITerminalServer _server;
        private readonly int _sessionId;
        private readonly string _userName;

        public TerminalServicesSession(ITerminalServer server, int sessionId)
        {
            _server = server;
            _sessionId = sessionId;
            _connectionState = SessionHelper.GetConnectionState(server.Handle, sessionId);
            _clientName = SessionHelper.GetClientName(server.Handle, sessionId);

            if (Environment.OSVersion.Version > new Version(6, 0))
            {
                // We can actually use documented APIs in Vista / Windows Server 2008+.
                WTSINFO info = SessionHelper.GetWtsInfo(server.Handle, sessionId);
                _connectTime = DateTime.FromFileTime(info.ConnectTime);
                _currentTime = DateTime.FromFileTime(info.CurrentTime);
                _disconnectTime = DateTime.FromFileTime(info.DisconnectTime);
                _lastInputTime = DateTime.FromFileTime(info.LastInputTime);
                _loginTime = DateTime.FromFileTime(info.LogonTime);
                _userName = info.UserName;
            }
            else
            {
                WINSTATIONINFORMATIONW wsInfo = SessionHelper.GetWinStationInformation(server.Handle, sessionId);
                _connectTime = SessionHelper.FileTimeToDateTime(wsInfo.ConnectTime);
                _currentTime = SessionHelper.FileTimeToDateTime(wsInfo.CurrentTime);
                _disconnectTime = SessionHelper.FileTimeToDateTime(wsInfo.DisconnectTime);
                _lastInputTime = SessionHelper.FileTimeToDateTime(wsInfo.LastInputTime);
                _loginTime = SessionHelper.FileTimeToDateTime(wsInfo.LoginTime);
                _userName = SessionHelper.GetUserName(server.Handle, sessionId);
            }
        }

        #region ITerminalServicesSession Members

        public string ClientName
        {
            get { return _clientName; }
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
    }
}