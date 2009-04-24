using System.Runtime.InteropServices;

namespace Cassia.Impl
{
    [StructLayout(LayoutKind.Sequential)]
    public struct WTS_SESSION_INFO
    {
        public int SessionID;
        [MarshalAs(UnmanagedType.LPTStr)]
        public string WinStationName;
        public ConnectionState State;
    }
}