using System;
using System.Collections.Generic;

namespace Cassia
{
    public interface ITerminalServer : IDisposable
    {
        IList<ITerminalServicesSession> GetSessions();
        ITerminalServicesSession GetSession(int sessionId);
        void Open();
        void Close();

        ITerminalServerHandle Handle
        {
            get;
        }

        bool IsOpen
        {
            get;
        }
    }
}