using System;

namespace Cassia
{
    [Flags]
    public enum RemoteMessageBoxOptions
    {
        None = 0,
        RightAligned = 0x00080000,
        RtlReading = 0x00100000,
        SetForeground = 0x00010000,
        TopMost = 0x00080000,
    }
}