using System;
using System.Runtime.InteropServices;
using FILETIME=System.Runtime.InteropServices.ComTypes.FILETIME;

namespace Cassia.Impl
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

        [DllImport("winsta.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int WinStationQueryInformation(IntPtr hServer, int sessionId, int information,
                                                            ref WINSTATIONINFORMATIONW buffer, int bufferLength,
                                                            ref int returnedLength);

        [DllImport("winsta.dll", CharSet = CharSet.Unicode, EntryPoint = "WinStationQueryInformationW",
            SetLastError = true)]
        public static extern int WinStationQueryInformationRemoteAddress(IntPtr hServer, int sessionId,
                                                                         WINSTATIONINFOCLASS information,
                                                                         ref WINSTATIONREMOTEADDRESS buffer,
                                                                         int bufferLength, out int returnedLength);

        [DllImport("wtsapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int WTSSendMessage(IntPtr hServer, int sessionId,
                                                [MarshalAs(UnmanagedType.LPTStr)] string title, int titleLength,
                                                [MarshalAs(UnmanagedType.LPTStr)] string message, int messageLength,
                                                int style, int timeout, out RemoteMessageBoxResult result, bool wait);

        [DllImport("wtsapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int WTSEnumerateServers([MarshalAs(UnmanagedType.LPTStr)] string pDomainName, int reserved,
                                                     int version, out IntPtr ppServerInfo, out int pCount);

        [DllImport("wtsapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int WTSEnumerateProcesses(IntPtr hServer, int reserved, int version,
                                                       out IntPtr ppProcessInfo, out int count);

        [DllImport("wtsapi32.dll", SetLastError = true)]
        public static extern int WTSShutdownSystem(IntPtr hServer, int shutdownFlag);

        [DllImport("wtsapi32.dll", SetLastError = true)]
        public static extern int WTSTerminateProcess(IntPtr hServer, int processId, int exitCode);

        [DllImport("ws2_32.dll")]
        public static extern ushort ntohs(ushort netValue);

        [DllImport("kernel32.dll")]
        public static extern int FileTimeToSystemTime(ref FILETIME fileTime, ref SYSTEMTIME systemTime);

        [DllImport("kernel32.dll")]
        public static extern int WTSGetActiveConsoleSessionId();

        [DllImport("wtsapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int WTSStartRemoteControlSession(string serverName, int targetSessionId, byte hotkeyVk,
                                                              short hotkeyModifiers);

        [DllImport("winsta.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern int WinStationShadow(IntPtr hServer, string serverName, int targetSessionId, int hotkeyVk,
                                                  int hotkeyModifier);

        [DllImport("winsta.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern int WinStationShadowStop(IntPtr hServer, int targetSessionId, bool wait);

        [DllImport("winsta.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern int WinStationConnectW(IntPtr hServer, int targetSessionId, int sourceSessionId,
                                                    string password, bool wait);

        [DllImport("wtsapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int WTSConnectSession(int sourceSessionId, int targetSessionId, string password, bool wait);

        [DllImport("wtsapi32.dll", SetLastError = true)]
        public static extern int WTSStopRemoteControlSession(int targetSessionId);
    }
}