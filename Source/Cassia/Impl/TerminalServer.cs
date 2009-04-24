using System;
using System.Collections.Generic;

namespace Cassia.Impl
{
    /// <summary>
    /// Default implementation of <see cref="ITerminalServer" />.
    /// </summary>
    public class TerminalServer : ITerminalServer
    {
        private readonly ITerminalServerHandle _handle;

        public TerminalServer(ITerminalServerHandle handle)
        {
            _handle = handle;
        }

        #region ITerminalServer Members

        public string ServerName
        {
            get { return _handle.ServerName; }
        }

        public bool IsOpen
        {
            get { return _handle.IsOpen; }
        }

        public ITerminalServerHandle Handle
        {
            get { return _handle; }
        }

        public IList<ITerminalServicesSession> GetSessions()
        {
            List<ITerminalServicesSession> results = new List<ITerminalServicesSession>();
            IList<WTS_SESSION_INFO> sessionInfos = NativeMethodsHelper.GetSessionInfos(Handle);
            foreach (WTS_SESSION_INFO sessionInfo in sessionInfos)
            {
                results.Add(new TerminalServicesSession(this, sessionInfo));
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
            _handle.Open();
        }

        public void Close()
        {
            _handle.Close();
        }

        public IList<ITerminalServicesProcess> GetProcesses()
        {
            List<ITerminalServicesProcess> processes = new List<ITerminalServicesProcess>();
            NativeMethodsHelper.ForEachProcessInfo(Handle,
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
            NativeMethodsHelper.ShutdownSystem(Handle, (int) type);
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
            }
        }
    }
}