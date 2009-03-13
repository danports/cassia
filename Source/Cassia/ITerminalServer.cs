using System.Collections.Generic;

namespace Cassia
{
    public interface ITerminalServer
    {
        ITerminalServicesSession CurrentSession { get; }
        IList<ITerminalServicesSession> GetSessions();
    }
}