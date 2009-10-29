using System;
using System.Collections.Generic;

namespace Cassia
{
    /// <summary>
    /// Top-level interface for enumerating and creating connections to terminal servers.
    /// </summary>
    public interface ITerminalServicesManager
    {
        /// <summary>
        /// The session in which the current process is running.
        /// </summary>
        ITerminalServicesSession CurrentSession { get; }

        /// <summary>
        /// The session to which the physical keyboard and mouse are connected.
        /// </summary>
        /// <remarks>
        /// Returns <c>null</c> if there is no active console session. This could occur if the 
        /// physical console session is in the process of being connected or disconnected.
        /// </remarks>
        ITerminalServicesSession ActiveConsoleSession { get; }

        /// <summary>
        /// Creates a connection to a remote terminal server.
        /// </summary>
        /// <param name="serverName">The name of the terminal server.</param>
        /// <returns>A <see cref="ITerminalServer" /> instance representing the requested server.</returns>
        ITerminalServer GetRemoteServer(string serverName);

        /// <summary>
        /// Creates a connection to the local terminal server.
        /// </summary>
        /// <returns>A <see cref="ITerminalServer" /> instance representing the local server.</returns>
        ITerminalServer GetLocalServer();

        /// <summary>
        /// Enumerates all terminal servers in a given domain.
        /// </summary>
        /// <param name="domainName">The name of the domain.</param>
        /// <returns>A list of terminal servers in the domain.</returns>
        IList<ITerminalServer> GetServers(string domainName);
    }
}