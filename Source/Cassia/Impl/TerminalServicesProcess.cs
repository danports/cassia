using System;
using System.Diagnostics;
using System.Security.Principal;

namespace Cassia.Impl
{
    /// <summary>
    /// Default implementation of <see cref="ITerminalServicesProcess" />.
    /// </summary>
    public class TerminalServicesProcess : ITerminalServicesProcess
    {
        private readonly int _processId;
        private readonly string _processName;
        private readonly SecurityIdentifier _securityIdentifier;
        private readonly ITerminalServer _server;
        private readonly int _sessionId;

        public TerminalServicesProcess(ITerminalServer server, WTS_PROCESS_INFO processInfo)
        {
            _server = server;
            _sessionId = processInfo.SessionId;
            _processId = processInfo.ProcessId;
            _processName = processInfo.ProcessName;
            // The SID could be null sometimes.
            // TODO: Windows 2008 R2 beta (locally) runs null for all processes except 
            // those owned by the current user; is this expected? (works fine from XP client)
            if (processInfo.UserSid != IntPtr.Zero)
            {
                _securityIdentifier = new SecurityIdentifier(processInfo.UserSid);
            }
        }

        #region ITerminalServicesProcess Members

        public ITerminalServer Server
        {
            get { return _server; }
        }

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

        public SecurityIdentifier SecurityIdentifier
        {
            get { return _securityIdentifier; }
        }

        public void Kill()
        {
            Kill(-1);
        }

        public void Kill(int exitCode)
        {
            NativeMethodsHelper.TerminateProcess(_server.Handle, _processId, exitCode);
        }

        public Process UnderlyingProcess
        {
            get
            {
                return
                    _server.ServerName == null
                        ? Process.GetProcessById(_processId)
                        : Process.GetProcessById(_processId, _server.ServerName);
            }
        }

        #endregion
    }
}