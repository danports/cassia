using System;
using System.Collections.Generic;

namespace Cassia
{
    /// <summary>
    /// Connection to a terminal server.
    /// </summary>
    /// <remarks>
    /// <see cref="Open" /> must be called before any operations can be performed on the terminal server.
    /// </remarks>
    public interface ITerminalServer : IDisposable
    {
        /// <summary>
        /// Underlying connection to the terminal server.
        /// </summary>
        ITerminalServerHandle Handle { get; }

        /// <summary>
        /// Returns <c>true</c> if a connection to the server is currently open.
        /// </summary>
        bool IsOpen { get; }

        /// <summary>
        /// The name of the terminal server.
        /// </summary>
        /// <remarks>
        /// It is not necessary to have a connection to the server open before 
        /// retrieving this value.
        /// </remarks>
        string ServerName { get; }

        /// <summary>
        /// Lists the sessions on the terminal server.
        /// </summary>
        /// <returns>A list of sessions.</returns>
        IList<ITerminalServicesSession> GetSessions();

        /// <summary>
        /// Retrieves information about a particular session on the server.
        /// </summary>
        /// <param name="sessionId">The ID of the session.</param>
        /// <returns>Information about the requested session.</returns>
        ITerminalServicesSession GetSession(int sessionId);

        /// <summary>
        /// Opens a connection to the server. Call this before attempting operations that access 
        /// information or perform operations on the server.
        /// </summary>
        void Open();

        /// <summary>
        /// Closes the connection to the server.
        /// </summary>
        void Close();

        /// <summary>
        /// Retrieves a list of processes running on the terminal server.
        /// </summary>
        /// <returns>A list of processes.</returns>
        IList<ITerminalServicesProcess> GetProcesses();

        /// <summary>
        /// Retrieves information about a particular process running on the server.
        /// </summary>
        /// <param name="processId">The ID of the process.</param>
        /// <returns>Information about the requested process.</returns>
        ITerminalServicesProcess GetProcess(int processId);

        /// <summary>
        /// Shuts down the terminal server.
        /// </summary>
        /// <param name="type">Type of shutdown requested.</param>
        void Shutdown(ShutdownType type);
    }
}