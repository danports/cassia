using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace Cassia.Impl
{
    [StructLayout(LayoutKind.Sequential)]
    public struct WTS_SESSION_ADDRESS
    {
        public AddressFamily AddressFamily;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
        public byte[] Address;
    }
}