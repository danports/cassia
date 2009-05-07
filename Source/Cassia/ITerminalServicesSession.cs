using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Principal;

namespace Cassia
{
    /// <summary>
    /// A session on a terminal server.
    /// </summary>
    /// <remarks>
    /// Note that many of the properties exposed by this interface may only be loaded on demand,
    /// so ensure that a connection to the terminal server is open 
    /// (by calling <see cref="ITerminalServer.Open()" />) before accessing properties or performing
    /// operations on a session.
    /// </remarks>
    public interface ITerminalServicesSession
    {
        /// <summary>
        /// The name of the machine last connected to this session.
        /// </summary>
        string ClientName { get; }

        /// <summary>
        /// The connection state of the session.
        /// </summary>
        ConnectionState ConnectionState { get; }

        /// <summary>
        /// The time at which the user connected to this session.
        /// </summary>
        /// <remarks>
        /// May be <c>null</c>, e.g. for a listening session.
        /// </remarks>
        DateTime? ConnectTime { get; }

        /// <summary>
        /// The current time in the session.
        /// </summary>
        /// <remarks>
        /// May be <c>null</c>, e.g. for a listening session.
        /// </remarks>
        DateTime? CurrentTime { get; }

        /// <summary>
        /// The time at which the user disconnected from this session.
        /// </summary>
        /// <remarks>
        /// May be <c>null</c>, e.g. if the user has never disconnected from the session.
        /// </remarks>
        DateTime? DisconnectTime { get; }

        /// <summary>
        /// The time at which this session last received input -- mouse movements, key presses, etc.
        /// </summary>
        /// <remarks>
        /// May be <c>null</c>, e.g. for a listening session that receives no user input.
        /// </remarks>
        DateTime? LastInputTime { get; }

        /// <summary>
        /// The time at which the user logged into this session for the first time.
        /// </summary>
        /// <remarks>
        /// May be <c>null</c>, e.g. for a listening session.
        /// </remarks>
        DateTime? LoginTime { get; }

        /// <summary>
        /// Time since the session last received user input.
        /// </summary>
        /// <remarks>This will return <c>TimeSpan.Zero</c> if there is no user connected to the 
        /// session, or the user is currently active.</remarks>
        TimeSpan IdleTime { get; }

        /// <summary>
        /// The ID of the session.
        /// </summary>
        int SessionId { get; }

        /// <summary>
        /// The name of the user account that last connected to the session.
        /// </summary>
        string UserName { get; }

        /// <summary>
        /// The domain of the user account that last connected to the session.
        /// </summary>
        string DomainName { get; }

        /// <summary>
        /// The user account that last connected to the session.
        /// </summary>
        NTAccount UserAccount { get; }

        /// <summary>
        /// The name of the session's window station.
        /// </summary>
        string WindowStationName { get; }

        /// <summary>
        /// The IP address reported by the client.
        /// </summary>
        /// <remarks>Note that this is not guaranteed to be the client's actual, remote 
        /// IP address -- if the client is behind a router with NAT, for example, the IP address
        /// reported will be the client's internal IP address on its LAN.</remarks>
        IPAddress ClientIPAddress { get; }

        /// <summary>
        /// The terminal server on which this session is located.
        /// </summary>
        ITerminalServer Server { get; }

        /// <summary>
        /// The build number of the client.
        /// </summary>
        /// <remarks>
        /// <para>Note that this does not include the major version, minor 
        /// version, or revision number -- it is only the build number. For example, the full file version 
        /// of the RDP 6 client on Windows XP is 6.0.6001.18000, so this property will return 6001
        /// for this client.</para>
        /// <para>May be zero, e.g. for a listening session.</para>
        /// </remarks>
        int ClientBuildNumber { get; }

        /// <summary>
        /// Information about a client's display.
        /// </summary>
        IClientDisplay ClientDisplay { get; }

        /// <summary>
        /// Directory on the client computer in which the client software is installed.
        /// </summary>
        /// <remarks>
        /// This is typically the full path to the RDP ActiveX control DLL on the client machine; e.g.
        /// <c>C:\WINDOWS\SYSTEM32\mstscax.dll</c>.
        /// </remarks>
        string ClientDirectory { get; }

        /// <summary>
        /// Client-specific hardware identifier.
        /// </summary>
        /// <remarks>
        /// This value is typically <c>0</c>.
        /// </remarks>
        int ClientHardwareId { get; }

        /// <overloads>
        /// <summary>
        /// Logs the session off, disconnecting any user that might be attached.
        /// </summary>
        /// </overloads>
        /// <summary>
        /// Logs the session off, disconnecting any user that might be attached.
        /// </summary>
        /// <remarks>The logoff takes place asynchronously; this method returns immediately. 
        /// This is the same as calling <c>Logoff(false)</c>.</remarks>
        void Logoff();

        /// <summary>
        /// Logs the session off, disconnecting any user that might be attached.
        /// </summary>
        /// <param name="synchronous">If <c>true</c>, waits until the session is fully logged off 
        /// before returning from the method. If <c>false</c>, returns immediately, even though
        /// the session may not be completely logged off yet.</param>
        void Logoff(bool synchronous);

        /// <overloads>
        /// <summary>
        /// Disconnects any attached user from the session.
        /// </summary>
        /// </overloads>
        /// <summary>
        /// Disconnects any attached user from the session.
        /// </summary>
        /// <remarks>The disconnection takes place asynchronously; this method returns immediately. 
        /// This is the same as calling <c>Disconnect(false)</c>.</remarks>
        void Disconnect();

        /// <summary>
        /// Disconnects any attached user from the session.
        /// </summary>
        /// <param name="synchronous">If <c>true</c>, waits until the session is fully disconnected 
        /// before returning from the method. If <c>false</c>, returns immediately, even though
        /// the session may not be completely disconnected yet.</param>
        void Disconnect(bool synchronous);

        /// <overloads>
        /// <summary>
        /// Displays a message box in the session.
        /// </summary>
        /// </overloads>
        /// <summary>
        /// Displays a message box in the session.
        /// </summary>
        /// <param name="text">The text to display in the message box.</param>
        void MessageBox(string text);

        /// <summary>
        /// Displays a message box in the session.
        /// </summary>
        /// <param name="text">The text to display in the message box.</param>
        /// <param name="caption">The caption of the message box.</param>
        void MessageBox(string text, string caption);

        /// <summary>
        /// Displays a message box in the session.
        /// </summary>
        /// <param name="text">The text to display in the message box.</param>
        /// <param name="caption">The caption of the message box.</param>
        /// <param name="icon">The icon to display in the message box.</param>
        void MessageBox(string text, string caption, RemoteMessageBoxIcon icon);

        /// <summary>
        /// Displays a message box in the session and returns the user's response to the message box.
        /// </summary>
        /// <param name="text">The text to display in the message box.</param>
        /// <param name="caption">The caption of the message box.</param>
        /// <param name="buttons">The buttons to display in the message box.</param>
        /// <param name="icon">The icon to display in the message box.</param>
        /// <param name="defaultButton">The button that should be selected by default in the message box.</param>
        /// <param name="options">Options for the message box.</param>
        /// <param name="timeout">The amount of time to wait for a response from the user 
        /// before closing the message box. The system will wait forever if this is set to <c>TimeSpan.Zero</c>.
        /// This will be treated as a integer number of seconds --
        /// specifying <c>TimeSpan.FromSeconds(2.5)</c> will produce the same result as 
        /// <c>TimeSpan.FromSeconds(2)</c>.</param>
        /// <param name="synchronous">If <c>true</c>, wait for and return the user's response to the
        /// message box. Otherwise, return immediately.</param>
        /// <returns>The user's response to the message box. If <paramref name="synchronous" />
        /// is <c>false</c>, the method will always return <see cref="RemoteMessageBoxResult.Asynchronous" />.
        /// If the timeout expired before the user responded to the message box, the result will be 
        /// <see cref="RemoteMessageBoxResult.Timeout" />.</returns>
        RemoteMessageBoxResult MessageBox(string text, string caption, RemoteMessageBoxButtons buttons,
                                          RemoteMessageBoxIcon icon, RemoteMessageBoxDefaultButton defaultButton,
                                          RemoteMessageBoxOptions options, TimeSpan timeout, bool synchronous);

        /// <summary>
        /// Retreives a list of processes running in this session.
        /// </summary>
        /// <returns>A list of processes.</returns>
        IList<ITerminalServicesProcess> GetProcesses();
    }
}