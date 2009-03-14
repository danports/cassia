using System;
using System.Collections.Generic;

namespace Cassia
{
    public class TerminalServer : ITerminalServer
    {
        private readonly string _serverName;
        private ITerminalServerHandle _handle;

        public TerminalServer() {}

        public TerminalServer(string serverName)
        {
            _serverName = serverName;
        }

        #region ITerminalServer Members

        public string ServerName
        {
            get { return _serverName; }
        }

        public bool IsOpen
        {
            get { return _handle != null; }
        }

        public ITerminalServerHandle Handle
        {
            get
            {
                if (_handle == null)
                {
                    throw new InvalidOperationException("Connection to server not open; did you forget to call Open?");
                }
                return _handle;
            }
        }

        public IList<ITerminalServicesSession> GetSessions()
        {
            List<ITerminalServicesSession> results = new List<ITerminalServicesSession>();
            IList<WTS_SESSION_INFO> sessionInfos = SessionHelper.GetSessionInfos(_handle);
            foreach (WTS_SESSION_INFO sessionInfo in sessionInfos)
            {
                results.Add(new TerminalServicesSession(this, sessionInfo.SessionID));
            }
            return results;
        }

        public ITerminalServicesSession GetSession(int sessionId)
        {
            return new TerminalServicesSession(this, sessionId);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Open()
        {
            _handle = new TerminalServerHandle(_serverName);
        }

        public void Close()
        {
            Dispose();
        }

        public IList<ITerminalServicesProcess> GetProcesses()
        {
            List<ITerminalServicesProcess> processes = new List<ITerminalServicesProcess>();
            SessionHelper.ForEachProcessInfo(Handle,
                                             delegate(WTS_PROCESS_INFO processInfo) { processes.Add(new TerminalServicesProcess(this, processInfo)); });
            return processes;
        }

        public ITerminalServicesProcess GetProcess(int processId)
        {
            foreach (ITerminalServicesProcess process in GetProcesses())
            {
                if (process.ProcessId == processId)
                {
                    return process;
                }
            }
            throw new InvalidOperationException("Process ID " + processId + " not found");
        }

        public void Shutdown(ShutdownType type)
        {
            SessionHelper.ShutdownSystem(Handle, (int) type);
        }

        #endregion

        ~TerminalServer()
        {
            Dispose(false);
        }

        protected void Dispose(bool disposing)
        {
            if (disposing)
            {
                _handle.Dispose();
                _handle = null;
            }
        }
    }
}