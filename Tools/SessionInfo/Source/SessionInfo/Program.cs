using System;
using Cassia;

namespace SessionInfo
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            string server = null;
            if (args.Length == 1)
            {
                server = args[0];
            }

            WriteServerInfo(server);
        }

        private static void WriteServerInfo(string server)
        {
            ITerminalServicesManager manager = new TerminalServicesManager();
            if (server == null)
            {
                Console.WriteLine("Current session:");
                WriteSessionInfo(manager.CurrentSession);
            }

            Console.WriteLine("All sessions:");
            foreach (ITerminalServicesSession session in manager.GetSessions(server))
            {
                WriteSessionInfo(session);
            }
        }

        private static void WriteSessionInfo(ITerminalServicesSession session)
        {
            Console.WriteLine("  Session ID: " + session.SessionId);
            Console.WriteLine("  User Name: " + session.UserName);
            Console.WriteLine("  State: " + session.ConnectionState);
            Console.WriteLine("  Logon Time: " + session.LoginTime);
            Console.WriteLine();
        }
    }
}