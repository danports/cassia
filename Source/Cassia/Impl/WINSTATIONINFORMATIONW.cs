using System.Runtime.InteropServices;
using FILETIME=System.Runtime.InteropServices.ComTypes.FILETIME;

namespace Cassia.Impl
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct WINSTATIONINFORMATIONW
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 70)]
        private byte[] Reserved1;
        public int SessionId;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        private byte[] Reserved2;
        public FILETIME ConnectTime;
        public FILETIME DisconnectTime;
        public FILETIME LastInputTime;
        public FILETIME LoginTime;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1096)]
        private byte[] Reserved3;
        public FILETIME CurrentTime;
    }
}