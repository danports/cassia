using System;
using Cassia.Tests.Server.InSession;

namespace Cassia.Tests.Server
{
    internal class Program
    {
        public const string InSessionSwitch = "/insession";

        private static void Main(string[] args)
        {
            var inSession = Array.Find(args, arg => String.Equals(arg, "/insession"));
            if (inSession != null)
            {
                Logger.InSessionLog("Starting...");
                try
                {
                    InSessionServer.Run();
                }
                catch (Exception ex)
                {
                    Logger.InSessionLog(ex.ToString());
                    throw;
                }
                Logger.InSessionLog("Stopping...");
                return;
            }

            Logger.MainServerLog("Starting...");
            ServiceRunner.Run(new TestServer(), args);
            Logger.MainServerLog("Stopping...");
        }
    }
}