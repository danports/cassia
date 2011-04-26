using System.Runtime.InteropServices;

namespace Cassia.Impl
{
    [StructLayout(LayoutKind.Sequential)]
    public struct CACHE_STATISTICS
    {
        private readonly short ProtocolType;
        private readonly short Length;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
        private readonly int[] Reserved;
    }
}