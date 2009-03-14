using System.Security.Principal;

namespace Cassia
{
    public class TerminalServicesProcess : ITerminalServicesProcess
    {
        private readonly int _processId;
        private readonly string _processName;
        private readonly int _sessionId;
        private readonly SecurityIdentifier _sid;

        public TerminalServicesProcess(WTS_PROCESS_INFO processInfo)
        {
            _sessionId = processInfo.SessionId;
            _processId = processInfo.ProcessId;
            _processName = processInfo.ProcessName;
            _sid = new SecurityIdentifier(processInfo.UserSid);
        }

        #region ITerminalServicesProcess Members

        public int SessionId
        {
            get { return _sessionId; }
        }

        public int ProcessId
        {
            get { return _processId; }
        }

        public string ProcessName
        {
            get { return _processName; }
        }

        public SecurityIdentifier Sid
        {
            get { return _sid; }
        }

        #endregion
    }
}