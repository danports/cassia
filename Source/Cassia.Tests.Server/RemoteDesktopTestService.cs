using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.ServiceModel;
using Cassia.Tests.Model;

namespace Cassia.Tests.Server
{
    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    public class RemoteDesktopTestService : IRemoteDesktopTestService
    {
        #region LogonProvider enum

        private enum LogonProvider
        {
            Default = 0
        }

        #endregion

        #region LogonType enum

        private enum LogonType
        {
            Interactive = 2,
            Network = 3,
            Batch = 4,
            Service = 5,
            Unlock = 7,
            NetworkCleartext = 8,
            NewCredentials = 9
        }

        #endregion

        private readonly ITerminalServicesManager _manager = new TerminalServicesManager();

        #region IRemoteDesktopTestService Members

        [OperationBehavior(Impersonation = ImpersonationOption.Required)]
        public void Disconnect(string serverName, int sessionId)
        {
            IntPtr token = IntPtr.Zero;
            if (LogonUser("", "", "", LogonType.Interactive, LogonProvider.Default, ref token) == 0)
            {
                throw new Win32Exception();
            }

            try
            {
                using (new WindowsIdentity(token).Impersonate())
                {
                    using (ITerminalServer server = GetServer(serverName))
                    {
                        server.Open();
                        server.GetSession(sessionId).Disconnect();
                    }
                }
            }
            finally
            {
                CloseHandle(token);
            }
        }

        [OperationBehavior(Impersonation = ImpersonationOption.Required)]
        public int GetLatestSessionId()
        {
            ITerminalServicesSession latest = null;
            foreach (ITerminalServicesSession session in _manager.GetLocalServer().GetSessions())
            {
                if (latest == null || latest.ConnectTime == null ||
                    (session.ConnectTime != null && session.ConnectTime > latest.ConnectTime))
                {
                    latest = session;
                }
            }
            return latest == null ? 0 : latest.SessionId;
        }

        [OperationBehavior(Impersonation = ImpersonationOption.Required)]
        public ConnectionState GetSessionState(int sessionId)
        {
            return _manager.GetLocalServer().GetSession(sessionId).ConnectionState;
        }

        [OperationBehavior(Impersonation = ImpersonationOption.Required)]
        public void Logoff(int sessionId)
        {
            _manager.GetLocalServer().GetSession(sessionId).Logoff();
        }

        [OperationBehavior(Impersonation = ImpersonationOption.Required)]
        public bool SessionExists(int sessionId)
        {
            try
            {
                ConnectionState state = _manager.GetLocalServer().GetSession(sessionId).ConnectionState;
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion

        [DllImport("Advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int LogonUser(string username, string domain, string password, LogonType logonType,
                                            LogonProvider logonProvider, ref IntPtr token);

        [DllImport("Kernel32.dll", SetLastError = true)]
        private static extern int CloseHandle(IntPtr handle);

        private ITerminalServer GetServer(string server)
        {
            return string.IsNullOrEmpty(server) ? _manager.GetLocalServer() : _manager.GetRemoteServer(server);
        }
    }
}