using System;

namespace Cassia
{
    /// <summary>
    /// Wraps the native terminal server handle.
    /// </summary>
    /// <remarks>
    /// You need use this interface only when you want to directly access the Windows terminal server
    /// handle to perform an operation that Cassia does not currently support.
    /// </remarks>
    public interface ITerminalServerHandle : IDisposable
    {
        /// <summary>
        /// The underlying terminal server handle provided by Windows in a call to WTSOpenServer.
        /// </summary>
        IntPtr Handle { get; }

        /// <summary>
        /// The name of the terminal server for this connection.
        /// </summary>
        string ServerName { get; }

        /// <summary>
        /// Gets a value indicating whether the connection to the server is currently open.
        /// </summary>
        bool IsOpen { get; }

        /// <summary>
        /// Gets a value indicating whether the handle is for the local server.
        /// </summary>
        bool Local { get; }

        /// <summary>
        /// Opens the terminal server handle.
        /// </summary>
        void Open();

        /// <summary>
        /// Closes the terminal server handle.
        /// </summary>
        void Close();
    }
}