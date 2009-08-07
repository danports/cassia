using System.Runtime.InteropServices;

namespace Cassia.Impl
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct CACHE_STATISTICS
    {
        short ProtocolType;
        short Length;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
        private int[] Reserved;
    }
}