using System;

namespace Cassia
{
    public class TerminalServicesSession : ITerminalServicesSession
    {
        private readonly ITerminalServer _server;
        private string _clientName;
        private WTS_CONNECTSTATE_CLASS _connectionState;
        private DateTime _connectTime;
        private DateTime _currentTime;
        private DateTime _disconnectTime;
        private DateTime _lastInputTime;
        private DateTime _loginTime;
        private int _sessionId;
        private string _userName;

        public TerminalServicesSession(ITerminalServer server)
        {
            _server = server;
        }

        #region ITerminalServicesSession Members

        public string ClientName
        {
            get { return _clientName; }
            set { _clientName = value; }
        }

        public WTS_CONNECTSTATE_CLASS ConnectionState
        {
            get { return _connectionState; }
            set { _connectionState = value; }
        }

        public DateTime ConnectTime
        {
            get { return _connectTime; }
            set { _connectTime = value; }
        }

        public DateTime CurrentTime
        {
            get { return _currentTime; }
            set { _currentTime = value; }
        }

        public DateTime DisconnectTime
        {
            get { return _disconnectTime; }
            set { _disconnectTime = value; }
        }

        public DateTime LastInputTime
        {
            get { return _lastInputTime; }
            set { _lastInputTime = value; }
        }

        public DateTime LoginTime
        {
            get { return _loginTime; }
            set { _loginTime = value; }
        }

        public int SessionId
        {
            get { return _sessionId; }
            set { _sessionId = value; }
        }

        public string UserName
        {
            get { return _userName; }
            set { _userName = value; }
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
                SessionHelper.SendMessage(_server.Handle, _sessionId, caption, text, style, timeoutSeconds,
                                          synchronous);
        }

        #endregion
    }
}