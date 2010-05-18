using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.ServiceProcess;

namespace Cassia.Tests
{
    public static class ServiceHelper
    {
        #region ScmAccessRights enum

        [Flags]
        public enum ScmAccessRights
        {
            AllAccess = 0xF003F,
            CreateService = 0x2,
            Connect = 0x1,
            EnumerateService = 0x4,
            Lock = 0x8,
            ModifyBootConfig = 0x20,
            QueryLockStatus = 0x10,
        }

        #endregion

        #region ServiceAccessRights enum

        public enum ServiceAccessRights
        {
            AllAccess = 0xF01FF,
            ChangeConfig = 0x2,
            EnumerateDependents = 0x8,
            Interrogate = 0x80,
            PauseContinue = 0x40,
            QueryConfig = 0x1,
            QueryStatus = 0x4,
            Start = 0x10,
            Stop = 0x20,
            UserDefinedControl = 0x100,
        }

        #endregion

        #region ServiceErrorControl enum

        public enum ServiceErrorControl
        {
            Critical = 0x3,
            Ignore = 0x0,
            Normal = 0x1,
            Severe = 0x2,
        }

        #endregion

        [DllImport("Advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern IntPtr OpenSCManager(string machineName, string databaseName,
                                                   ScmAccessRights desiredAccess);

        [DllImport("Advapi32.dll", SetLastError = true)]
        private static extern int CloseServiceHandle(IntPtr handle);

        [DllImport("Advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern IntPtr CreateService(IntPtr hScManager, string serviceName, string displayName,
                                                   ServiceAccessRights desiredAccess, ServiceType serviceType,
                                                   ServiceStartMode startType, ServiceErrorControl errorControl,
                                                   string binaryPathName, string loadOrderGroup, IntPtr tagId,
                                                   string dependencies, string serviceStartName, string password);

        [DllImport("Advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern IntPtr OpenService(IntPtr hScManager, string serviceNam, ServiceAccessRights desiredAccess);

        [DllImport("Advapi32.dll", SetLastError = true)]
        private static extern int DeleteService(IntPtr hService);

        public static ServiceController Create(string machineName, string serviceName, string displayName,
                                               ServiceType serviceType, ServiceStartMode startType,
                                               ServiceErrorControl errorControl, string binaryPathName,
                                               string loadOrderGroup, string[] dependencies, string serviceStartName,
                                               string password)
        {
            IntPtr hScManager = OpenSCManager(machineName, null, ScmAccessRights.CreateService);
            if (hScManager == IntPtr.Zero)
            {
                throw new Win32Exception();
            }
            try
            {
                string dependencyList = dependencies == null || dependencies.Length == 0
                                            ? null
                                            : string.Join("\0", dependencies);
                IntPtr hService = CreateService(hScManager, serviceName, displayName, ServiceAccessRights.AllAccess,
                                                serviceType, startType, errorControl, binaryPathName, loadOrderGroup,
                                                IntPtr.Zero, dependencyList, serviceStartName, password);
                if (hService == IntPtr.Zero)
                {
                    throw new Win32Exception();
                }
                CloseServiceHandle(hService);

                return new ServiceController(serviceName, machineName);
            }
            finally
            {
                CloseServiceHandle(hScManager);
            }
        }

        public static void Delete(ServiceController service)
        {
            IntPtr hScManager = OpenSCManager(service.MachineName, null, ScmAccessRights.AllAccess);
            if (hScManager == IntPtr.Zero)
            {
                throw new Win32Exception();
            }
            try
            {
                IntPtr hService = OpenService(hScManager, service.ServiceName, ServiceAccessRights.AllAccess);
                if (hService == IntPtr.Zero)
                {
                    throw new Win32Exception();
                }

                try
                {
                    if (DeleteService(hService) == 0)
                    {
                        throw new Win32Exception();
                    }
                }
                finally
                {
                    CloseServiceHandle(hService);
                }
            }
            finally
            {
                CloseServiceHandle(hScManager);
            }
        }
    }
}