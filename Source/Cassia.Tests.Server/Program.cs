using System;
using Cassia.Tests.Server.InSession;

namespace Cassia.Tests.Server
{
    internal class Program
    {
        public const string InSessionSwitch = "/insession";

        private static void Main(string[] args)
        {
            if (Array.Find(args, arg => String.Equals(arg, InSessionSwitch)) == null)
            {
                ServiceRunner.Run(new TestServer(), args);
                return;
            }

            Logger.Log("Starting...");
            try
            {
                InSessionServer.Run();
            }
            catch (Exception ex)
            {
                Logger.Log(ex.ToString());
                throw;
            }
            Logger.Log("Stopping...");
        }
    }
}