using System.Runtime.InteropServices;
using FILETIME = System.Runtime.InteropServices.ComTypes.FILETIME;

namespace Cassia.Impl
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct WTSINFOEX_LEVEL1
    {
        public int SessionId;
        public ConnectionState SessionState;
        public int SessionFlags;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 33)]
        public string WinStationName;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 21)]
        public string UserName;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 18)]
        public string DomainName;

        public int Unknown;
        public FILETIME LogonTime;
        public FILETIME ConnectTime;
        public FILETIME DisconnectTime;
        public FILETIME LastInputTime;
        public FILETIME CurrentTime;
        public int IncomingBytes;
        public int OutgoingBytes;
        public int IncomingFrames;
        public int OutgoingFrames;
        public int IncomingCompressedBytes;
        public int OutgoingCompressedBytes;
    }
}