using System.Diagnostics;
using System.Security.Principal;

namespace Cassia
{
    public class TerminalServicesProcess : ITerminalServicesProcess
    {
        private readonly int _processId;
        private readonly string _processName;
        private readonly ITerminalServer _server;
        private readonly int _sessionId;
        private readonly SecurityIdentifier _sid;

        public TerminalServicesProcess(ITerminalServer server, WTS_PROCESS_INFO processInfo)
        {
            _server = server;
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

        public void Kill()
        {
            Kill(-1);
        }

        public void Kill(int exitCode)
        {
            SessionHelper.TerminateProcess(_server.Handle, _processId, exitCode);
        }

        public Process GetProcessObject()
        {
            return
                _server.ServerName == null
                    ? Process.GetProcessById(_processId)
                    : Process.GetProcessById(_processId, _server.ServerName);
        }

        #endregion
    }
}