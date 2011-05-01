using System.Diagnostics;

namespace Cassia.Tests.Server.ServiceComponents
{
    public class EventLogLogger : ILogger
    {
        private readonly EventLog _eventLog;

        public EventLogLogger(EventLog eventLog)
        {
            _eventLog = eventLog;
        }

        #region ILogger Members

        public void Log(string text)
        {
            _eventLog.WriteEntry(text);
        }

        #endregion
    }
}