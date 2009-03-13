using System.Collections.Generic;

namespace Cassia
{
    public interface ITerminalServer
    {
        IList<ITerminalServicesSession> GetSessions();
        ITerminalServicesSession GetSession(int sessionId);
    }
}