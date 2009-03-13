using System.Collections.Generic;

namespace Cassia
{
    public class TerminalServicesManager : ITerminalServicesManager
    {
        #region ITerminalServicesManager Members

        public ITerminalServicesSession CurrentSession
        {
            get { return new TerminalServer().CurrentSession; }
        }

        public IList<ITerminalServicesSession> GetSessions(string serverName)
        {
            return new TerminalServer(serverName).GetSessions();
        }

        public IList<ITerminalServicesSession> GetSessions()
        {
            return GetSessions(null);
        }

        #endregion
    }
}