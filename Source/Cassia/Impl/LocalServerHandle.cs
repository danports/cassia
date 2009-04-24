using System;

namespace Cassia.Impl
{
    public class LocalServerHandle : ITerminalServerHandle
    {
        public IntPtr Handle
        {
            get { return NativeMethods.LocalServerHandle; }
        }

        public string ServerName
        {
            get { return null; }
        }

        public bool IsOpen
        {
            get { return true; }
        }

        public void Open() {}

        public void Close() {}

        public void Dispose() {}
    }
}