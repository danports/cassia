using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Security.Principal;
using Cassia.Tests.Model;

namespace Cassia.Tests.Server
{
    public static class ImpersonationHelper
    {
        #region LogonProvider enum

        public enum LogonProvider
        {
            Default = 0
        }

        #endregion

        #region LogonType enum

        public enum LogonType
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

        [DllImport("Advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int LogonUser(string username, string domain, string password, LogonType logonType,
                                            LogonProvider logonProvider, ref IntPtr token);

        [DllImport("Kernel32.dll", SetLastError = true)]
        private static extern int CloseHandle(IntPtr handle);

        public static IDisposable Impersonate(ConnectionDetails connection)
        {
            var token = IntPtr.Zero;
            if (
                LogonUser(connection.Username, connection.Domain, connection.Password, LogonType.Interactive,
                          LogonProvider.Default, ref token) == 0)
            {
                throw new Win32Exception();
            }
            return new ImpersonationContext(token);
        }

        #region Nested type: ImpersonationContext

        private class ImpersonationContext : IDisposable
        {
            private readonly WindowsImpersonationContext _impersonationContext;
            private readonly IntPtr _token;

            public ImpersonationContext(IntPtr token)
            {
                _token = token;
                _impersonationContext = new WindowsIdentity(token).Impersonate();
            }

            #region IDisposable Members

            public void Dispose()
            {
                _impersonationContext.Dispose();
                CloseHandle(_token);
            }

            #endregion
        }

        #endregion
    }
}