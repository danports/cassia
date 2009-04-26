using System;
using System.Collections.Generic;

namespace Cassia
{
    /// <summary>
    /// Top-level class that enumerates and creates connections to terminal servers.
    /// </summary>
    public interface ITerminalServicesManager
    {
        /// <summary>
        /// Provides information about the session in which the current process is running.
        /// </summary>
        ITerminalServicesSession CurrentSession { get; }

        /// <overloads>
        /// <summary>
        /// Lists the sessions on a given terminal server. Obsolete; use 
        /// <see cref="ITerminalServer.GetSessions()" />.
        /// </summary>
        /// </overloads>
        /// <param name="serverName">The name of the terminal server.</param>
        /// <returns>A list of sessions on the terminal server.</returns>
        [Obsolete("Use ITerminalServer.GetSessions() instead.")]
        IList<ITerminalServicesSession> GetSessions(string serverName);

        /// <returns>A list of sessions on the local server.</returns>
        [Obsolete("Use ITerminalServer.GetSessions() instead.")]
        IList<ITerminalServicesSession> GetSessions();

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