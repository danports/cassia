using System;

namespace Cassia
{
    public interface ITerminalServerHandle : IDisposable
    {
        IntPtr Handle { get; }
    }
}