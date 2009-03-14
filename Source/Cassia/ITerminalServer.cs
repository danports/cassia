using System;
using System.Collections.Generic;

namespace Cassia
{
    public interface ITerminalServer : IDisposable
    {
        ITerminalServerHandle Handle { get; }
        bool IsOpen { get; }
        string ServerName { get; }
        IList<ITerminalServicesSession> GetSessions();
        ITerminalServicesSession GetSession(int sessionId);
        void Open();
        void Close();
        IList<ITerminalServicesProcess> GetProcesses();
        ITerminalServicesProcess GetProcess(int processId);
        void Shutdown(ShutdownType type);
    }
}