using System.Runtime.InteropServices;

namespace Cassia.Impl
{
    [StructLayout(LayoutKind.Sequential)]
    public struct WTS_SERVER_INFO
    {
        [MarshalAs(UnmanagedType.LPTStr)]
        public string ServerName;
    }
}