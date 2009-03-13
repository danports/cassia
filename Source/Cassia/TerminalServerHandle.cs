using System;
using System.ComponentModel;

namespace Cassia
{
    public class TerminalServerHandle : IDisposable
    {
        private readonly IntPtr _serverPtr;

        public TerminalServerHandle(string serverName)
        {
            if (string.IsNullOrEmpty(serverName))
            {
                _serverPtr = NativeMethods.LocalServerHandle;
            }
            else
            {
                _serverPtr = NativeMethods.WTSOpenServer(serverName);
                if (_serverPtr == IntPtr.Zero)
                {
                    // Failed to connect, possibly because Terminal Services is not running on the remote machine.
                    throw new Win32Exception();
                }
            }
        }

        public IntPtr Handle
        {
            get { return _serverPtr; }
        }

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        protected void Dispose(bool disposing)
        {
            if (_serverPtr != NativeMethods.LocalServerHandle)
            {
                NativeMethods.WTSCloseServer(_serverPtr);
            }
        }

        ~TerminalServerHandle()
        {
            Dispose(false);
        }
    }
}