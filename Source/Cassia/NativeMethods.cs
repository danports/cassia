using System;
using System.Runtime.InteropServices;
using Cassia;

namespace Cassia
{
    [CLSCompliant(false)]
    public static class NativeMethods
    {
        public const uint CurrentSessionId = uint.MaxValue;
        public static readonly IntPtr LocalServerHandle = IntPtr.Zero;

        [DllImport("Wtsapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool WTSQuerySessionInformation(IntPtr hServer, uint sessionId, WTS_INFO_CLASS wtsInfoClass,
                                                             out IntPtr buffer, out uint bytesReturned);

        [DllImport("wtsapi32.dll", SetLastError = true)]
        public static extern Int32 WTSEnumerateSessions(IntPtr hServer, uint Reserved, uint Version,
                                                        out IntPtr sessionInfo, out uint count);

        [DllImport("wtsapi32.dll")]
        public static extern void WTSFreeMemory(IntPtr memory);

        [DllImport("wtsapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr WTSOpenServer(string serverName);

        [DllImport("wtsapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern void WTSCloseServer(IntPtr hServer);

        [DllImport("winsta.dll")]
        public static extern int WinStationQueryInformationW(IntPtr hServer, uint sessionId, uint information,
                                                             ref WINSTATIONINFORMATIONW buffer, uint bufferLength,
                                                             ref uint returnedLength);
    }
}