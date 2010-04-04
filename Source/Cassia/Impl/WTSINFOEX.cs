using System.Runtime.InteropServices;

namespace Cassia.Impl
{
    [StructLayout(LayoutKind.Sequential)]
    public struct WTSINFOEX
    {
        public int Level;
        public int Unknown;
        public WTSINFOEX_LEVEL1 Data;
    }
}