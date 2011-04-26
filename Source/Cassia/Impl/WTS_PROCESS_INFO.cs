using System;
using System.Runtime.InteropServices;

namespace Cassia.Impl
{
    [StructLayout(LayoutKind.Sequential)]
    public struct WTS_PROCESS_INFO
    {
        public int SessionId;
        public int ProcessId;

        [MarshalAs(UnmanagedType.LPTStr)]
        public string ProcessName;

        public IntPtr UserSid;
    }
}