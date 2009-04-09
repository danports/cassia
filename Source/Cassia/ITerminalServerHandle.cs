using System;

namespace Cassia
{
    /// <summary>
    /// Wraps the native terminal server handle.
    /// </summary>
    public interface ITerminalServerHandle : IDisposable
    {
        /// <summary>
        /// The underlying terminal server handle provided by Windows in a call to WTSOpenServer.
        /// </summary>
        IntPtr Handle { get; }
    }
}