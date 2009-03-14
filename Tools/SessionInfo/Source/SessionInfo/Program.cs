using System;
using Cassia;

namespace SessionInfo
{
    internal class Program
    {
        private static readonly ITerminalServicesManager _manager = new TerminalServicesManager();

        private static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                ShowCurrentSession();
                return;
            }
            switch (args[0].ToLower())
            {
                case "current":
                    ShowCurrentSession();
                    return;
                case "get":
                    GetSessionInfo(args);
                    return;
                case "list":
                    ListSessions(args);
                    return;
                case "logoff":
                    LogoffSession(args);
                    return;
                case "disconnect":
                    DisconnectSession(args);
                    return;
            }
            Console.WriteLine("Unknown command: " + args[0]);
        }

        private static void GetSessionInfo(string[] args)
        {
            if (args.Length < 3)
            {
                Console.WriteLine("Usage: SessionInfo get [server] [session id]");
                return;
            }
            int sessionId = int.Parse(args[2]);
            using (ITerminalServer server = GetServerFromName(args[1]))
            {
                server.Open();
                WriteSessionInfo(server.GetSession(sessionId));
            }
        }

        private static void ListSessions(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Usage: SessionInfo list [server]");
                return;
            }
            using (ITerminalServer server = GetServerFromName(args[1]))
            {
                server.Open();
                foreach (ITerminalServicesSession session in server.GetSessions())
                {
                    WriteSessionInfo(session);
                }
            }
        }

        private static void ShowCurrentSession()
        {
            WriteSessionInfo(_manager.CurrentSession);
        }

        private static void LogoffSession(string[] args)
        {
            if (args.Length < 3)
            {
                Console.WriteLine("Usage: SessionInfo disconnect [server] [session id]");
                return;
            }
            string serverName = args[1];
            int sessionId = int.Parse(args[2]);
            using (ITerminalServer server = GetServerFromName(serverName))
            {
                server.Open();
                ITerminalServicesSession session = server.GetSession(sessionId);
                session.Logoff();
            }
        }

        private static ITerminalServer GetServerFromName(string serverName)
        {
            return
                (serverName.Equals("local", StringComparison.InvariantCultureIgnoreCase)
                     ? _manager.GetLocalServer()
                     : _manager.GetRemoteServer(serverName));
        }

        private static void DisconnectSession(string[] args)
        {
            if (args.Length < 3)
            {
                Console.WriteLine("Usage: SessionInfo disconnect [server] [session id]");
                return;
            }
            string serverName = args[1];
            int sessionId = int.Parse(args[2]);
            using (ITerminalServer server = GetServerFromName(serverName))
            {
                server.Open();
                ITerminalServicesSession session = server.GetSession(sessionId);
                session.Disconnect();
            }
        }

        private static void WriteSessionInfo(ITerminalServicesSession session)
        {
            Console.WriteLine("Session ID: " + session.SessionId);
            Console.WriteLine("User Name: " + session.UserName);
            Console.WriteLine("State: " + session.ConnectionState);
            Console.WriteLine("Logon Time: " + session.LoginTime);
            Console.WriteLine();
        }
    }
}