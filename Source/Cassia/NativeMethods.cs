using System;
using System.Runtime.InteropServices;

namespace Cassia
{
    internal static class NativeMethods
    {
        public const int CurrentSessionId = -1;
        public static readonly IntPtr LocalServerHandle = IntPtr.Zero;

        [DllImport("Wtsapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool WTSQuerySessionInformation(IntPtr hServer, int sessionId, WTS_INFO_CLASS wtsInfoClass,
                                                             out IntPtr buffer, out int bytesReturned);

        [DllImport("wtsapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern Int32 WTSEnumerateSessions(IntPtr hServer, int reserved, int version,
                                                        out IntPtr sessionInfo, out int count);

        [DllImport("wtsapi32.dll")]
        public static extern void WTSFreeMemory(IntPtr memory);

        [DllImport("wtsapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr WTSOpenServer(string serverName);

        [DllImport("wtsapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern void WTSCloseServer(IntPtr hServer);

        [DllImport("wtsapi32.dll", SetLastError = true)]
        public static extern int WTSLogoffSession(IntPtr hServer, int sessionId, bool wait);

        [DllImport("wtsapi32.dll", SetLastError = true)]
        public static extern int WTSDisconnectSession(IntPtr hServer, int sessionId, bool wait);

        [DllImport("winsta.dll", CharSet = CharSet.Auto)]
        public static extern int WinStationQueryInformation(IntPtr hServer, int sessionId, int information,
                                                            ref WINSTATIONINFORMATIONW buffer, int bufferLength,
                                                            ref int returnedLength);

        [DllImport("Wtsapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int WTSSendMessage(IntPtr hServer, int sessionId,
                                                [MarshalAs(UnmanagedType.LPTStr)] string title, int titleLength,
                                                [MarshalAs(UnmanagedType.LPTStr)] string message, int messageLength,
                                                int style, int timeout, out RemoteMessageBoxResult result, bool wait);

        [DllImport("Wtsapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int WTSEnumerateServers([MarshalAs(UnmanagedType.LPTStr)] string pDomainName, int reserved,
                                                     int version, out IntPtr ppServerInfo, out int pCount);

        [DllImport("Wtsapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int WTSEnumerateProcesses(IntPtr hServer, int reserved, int version,
                                                       out IntPtr ppProcessInfo, out int count);

        [DllImport("wtsapi32.dll", SetLastError = true)]
        public static extern int WTSShutdownSystem(IntPtr hServer, int shutdownFlag);

        [DllImport("wtsapi32.dll", SetLastError = true)]
        public static extern int WTSTerminateProcess(IntPtr hServer, int processId, int exitCode);
    }
}