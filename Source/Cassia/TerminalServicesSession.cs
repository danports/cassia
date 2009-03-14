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
        private long _sessionId;
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

        public long SessionId
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
            SessionHelper.LogoffSession(_server.Handle, (uint) _sessionId, synchronous);
        }

        public void Disconnect()
        {
            Disconnect(false);
        }

        public void Disconnect(bool synchronous)
        {
            SessionHelper.DisconnectSession(_server.Handle, (uint) _sessionId, synchronous);
        }

        #endregion
    }
}