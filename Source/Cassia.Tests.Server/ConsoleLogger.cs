using System;

namespace Cassia.Tests.Server
{
    public class ConsoleLogger : ILogger
    {
        #region ILogger Members

        public void Log(string text)
        {
            Console.WriteLine(DateTime.Now + ": " + text);
        }

        #endregion
    }
}