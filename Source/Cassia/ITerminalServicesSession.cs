using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Principal;

namespace Cassia
{
    /// <summary>
    ///     A session on a terminal server.
    /// </summary>
    /// <remarks>
    ///     Note that many of the properties exposed by this interface may only be loaded on demand,
    ///     so ensure that a connection to the terminal server is open
    ///     (by calling <see cref="ITerminalServer.Open()" />) before accessing properties or performing
    ///     operations on a session.
    /// </remarks>
    public interface ITerminalServicesSession
    {
        /// <summary>
        ///     The name of the machine last connected to this session.
        /// </summary>
        string ClientName { get; }

        /// <summary>
        ///     The connection state of the session.
        /// </summary>
        ConnectionState ConnectionState { get; }

        /// <summary>
        ///     The time at which the user connected to this session.
        /// </summary>
        /// <remarks>
        ///     May be <c>null</c>, e.g. for a listening session.
        /// </remarks>
        DateTime? ConnectTime { get; }

        /// <summary>
        ///     The current time in the session.
        /// </summary>
        /// <remarks>
        ///     May be <c>null</c>, e.g. for a listening session.
        /// </remarks>
        DateTime? CurrentTime { get; }

        /// <summary>
        ///     The time at which the user disconnected from this session.
        /// </summary>
        /// <remarks>
        ///     May be <c>null</c>, e.g. if the user has never disconnected from the session.
        /// </remarks>
        DateTime? DisconnectTime { get; }

        /// <summary>
        ///     The time at which this session last received input -- mouse movements, key presses, etc.
        /// </summary>
        /// <remarks>
        ///     May be <c>null</c>, e.g. for a listening session that receives no user input.
        /// </remarks>
        DateTime? LastInputTime { get; }

        /// <summary>
        ///     The time at which the user logged into this session for the first time.
        /// </summary>
        /// <remarks>
        ///     May be <c>null</c>, e.g. for a listening session.
        /// </remarks>
        DateTime? LoginTime { get; }

        /// <summary>
        ///     Length of time that the session has been idle.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         For connected sessions, this will return the time since the session
        ///         last received user input.
        ///     </para>
        ///     <para>
        ///         For disconnected sessions, this will return the length of time that the user
        ///         has been disconnected from the session.
        ///     </para>
        ///     <para>This will return <c>TimeSpan.Zero</c> if the idle time could not be determined.</para>
        /// </remarks>
        TimeSpan IdleTime { get; }

        /// <summary>
        ///     The ID of the session.
        /// </summary>
        int SessionId { get; }

        /// <summary>
        ///     The name of the user account that last connected to the session.
        /// </summary>
        string UserName { get; }

        /// <summary>
        ///     The domain of the user account that last connected to the session.
        /// </summary>
        string DomainName { get; }

        /// <summary>
        ///     The user account that last connected to the session.
        /// </summary>
        NTAccount UserAccount { get; }

        /// <summary>
        ///     The name of the session's window station.
        /// </summary>
        string WindowStationName { get; }

        /// <summary>
        ///     The IP address reported by the client.
        /// </summary>
        /// <remarks>
        ///     Note that this is not guaranteed to be the client's actual, remote
        ///     IP address -- if the client is behind a router with NAT, for example, the IP address
        ///     reported will be the client's internal IP address on its LAN.
        /// </remarks>
        IPAddress ClientIPAddress { get; }

        /// <summary>
        ///     The virtual IP address assigned to this session.
        /// </summary>
        /// <remarks>
        ///     This is only supported on Windows Server 2008 R2/Windows 7 and later. It will throw an exception if the session
        ///     does not have a virtual IP address.
        /// </remarks>
        IPAddress SessionIPAddress { get; }

        /// <summary>
        ///     The terminal server on which this session is located.
        /// </summary>
        ITerminalServer Server { get; }

        /// <summary>
        ///     The build number of the client.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         Note that this does not include the major version, minor
        ///         version, or revision number -- it is only the build number. For example, the full file version
        ///         of the RDP 6 client on Windows XP is 6.0.6001.18000, so this property will return 6001
        ///         for this client.
        ///     </para>
        ///     <para>May be zero, e.g. for a listening session.</para>
        /// </remarks>
        int ClientBuildNumber { get; }

        /// <summary>
        ///     Information about a client's display.
        /// </summary>
        IClientDisplay ClientDisplay { get; }

        /// <summary>
        ///     Directory on the client computer in which the client software is installed.
        /// </summary>
        /// <remarks>
        ///     This is typically the full path to the RDP ActiveX control DLL on the client machine; e.g.
        ///     <c>C:\WINDOWS\SYSTEM32\mstscax.dll</c>.
        /// </remarks>
        string ClientDirectory { get; }

        /// <summary>
        ///     Client-specific hardware identifier.
        /// </summary>
        /// <remarks>
        ///     This value is typically <c>0</c>.
        /// </remarks>
        int ClientHardwareId { get; }

        /// <summary>
        ///     Client-specific product identifier.
        /// </summary>
        /// <remarks>
        ///     This value is typically <c>1</c> for the standard RDP client.
        /// </remarks>
        short ClientProductId { get; }

        /// <summary>
        ///     The protocol that the client is using to connect to the session.
        /// </summary>
        ClientProtocolType ClientProtocolType { get; }

        /// <summary>
        ///     The working directory used when launching the initial program.
        /// </summary>
        /// <remarks>
        ///     This property may throw an exception for the console session (where
        ///     <see cref="ClientProtocolType" /> is <see cref="Cassia.ClientProtocolType.Console" />).
        /// </remarks>
        string WorkingDirectory { get; }

        /// <summary>
        ///     The initial program run when the session started.
        /// </summary>
        /// <remarks>
        ///     This property may throw an exception for the console session (where
        ///     <see cref="ClientProtocolType" /> is <see cref="Cassia.ClientProtocolType.Console" />).
        /// </remarks>
        string InitialProgram { get; }

        /// <summary>
        ///     The remote endpoint (IP address and port) of the client connected to the session.
        /// </summary>
        /// <remarks>
        ///     This property currently supports only IPv4 addresses, and will be <c>null</c> if
        ///     no client is connected to the session.
        /// </remarks>
        EndPoint RemoteEndPoint { get; }

        /// <summary>
        ///     Name of the published application that this session is running.
        /// </summary>
        /// <remarks>
        ///     This property may throw an exception for the console session (where
        ///     <see cref="ClientProtocolType" /> is <see cref="Cassia.ClientProtocolType.Console" />).
        /// </remarks>
        string ApplicationName { get; }

        /// <summary>
        ///     Gets a value indicating whether this session is running on the local terminal server.
        /// </summary>
        bool Local { get; }

        /// <summary>
        ///     Incoming protocol statistics for the session.
        /// </summary>
        IProtocolStatistics IncomingStatistics { get; }

        /// <summary>
        ///     Outgoing protocol statistics for the session.
        /// </summary>
        IProtocolStatistics OutgoingStatistics { get; }

        /// <overloads>
        ///     <summary>
        ///         Logs the session off, disconnecting any user that might be attached.
        ///     </summary>
        /// </overloads>
        /// <summary>
        ///     Logs the session off, disconnecting any user that might be attached.
        /// </summary>
        /// <remarks>
        ///     The logoff takes place synchronously; this method returns after the operation is complete.
        ///     This is the same as calling <c>Logoff(true)</c>.
        /// </remarks>
        void Logoff();

        /// <summary>
        ///     Logs the session off, disconnecting any user that might be attached.
        /// </summary>
        /// <param name="synchronous">
        ///     If <c>true</c>, waits until the session is fully logged off
        ///     before returning from the method. If <c>false</c>, returns immediately, even though
        ///     the session may not be completely logged off yet.
        /// </param>
        void Logoff(bool synchronous);

        /// <overloads>
        ///     <summary>
        ///         Disconnects any attached user from the session.
        ///     </summary>
        /// </overloads>
        /// <summary>
        ///     Disconnects any attached user from the session.
        /// </summary>
        /// <remarks>
        ///     The disconnection takes place synchronously; this method returns after the operation is complete.
        ///     This is the same as calling <c>Disconnect(true)</c>.
        /// </remarks>
        void Disconnect();

        /// <summary>
        ///     Disconnects any attached user from the session.
        /// </summary>
        /// <param name="synchronous">
        ///     If <c>true</c>, waits until the session is fully disconnected
        ///     before returning from the method. If <c>false</c>, returns immediately, even though
        ///     the session may not be completely disconnected yet.
        /// </param>
        void Disconnect(bool synchronous);

        /// <overloads>
        ///     <summary>
        ///         Displays a message box in the session.
        ///     </summary>
        /// </overloads>
        /// <summary>
        ///     Displays a message box in the session.
        /// </summary>
        /// <param name="text">The text to display in the message box.</param>
        void MessageBox(string text);

        /// <summary>
        ///     Displays a message box in the session.
        /// </summary>
        /// <param name="text">The text to display in the message box.</param>
        /// <param name="caption">The caption of the message box.</param>
        void MessageBox(string text, string caption);

        /// <summary>
        ///     Displays a message box in the session.
        /// </summary>
        /// <param name="text">The text to display in the message box.</param>
        /// <param name="caption">The caption of the message box.</param>
        /// <param name="icon">The icon to display in the message box.</param>
        void MessageBox(string text, string caption, RemoteMessageBoxIcon icon);

        /// <summary>
        ///     Displays a message box in the session and returns the user's response to the message box.
        /// </summary>
        /// <param name="text">The text to display in the message box.</param>
        /// <param name="caption">The caption of the message box.</param>
        /// <param name="buttons">The buttons to display in the message box.</param>
        /// <param name="icon">The icon to display in the message box.</param>
        /// <param name="defaultButton">The button that should be selected by default in the message box.</param>
        /// <param name="options">Options for the message box.</param>
        /// <param name="timeout">
        ///     The amount of time to wait for a response from the user
        ///     before closing the message box. The system will wait forever if this is set to <c>TimeSpan.Zero</c>.
        ///     This will be treated as a integer number of seconds --
        ///     specifying <c>TimeSpan.FromSeconds(2.5)</c> will produce the same result as
        ///     <c>TimeSpan.FromSeconds(2)</c>.
        /// </param>
        /// <param name="synchronous">
        ///     If <c>true</c>, wait for and return the user's response to the
        ///     message box. Otherwise, return immediately.
        /// </param>
        /// <returns>
        ///     The user's response to the message box. If <paramref name="synchronous" />
        ///     is <c>false</c>, the method will always return <see cref="RemoteMessageBoxResult.Asynchronous" />.
        ///     If the timeout expired before the user responded to the message box, the result will be
        ///     <see cref="RemoteMessageBoxResult.Timeout" />.
        /// </returns>
        RemoteMessageBoxResult MessageBox(string text, string caption, RemoteMessageBoxButtons buttons,
            RemoteMessageBoxIcon icon, RemoteMessageBoxDefaultButton defaultButton, RemoteMessageBoxOptions options,
            TimeSpan timeout, bool synchronous);

        /// <summary>
        ///     Retreives a list of processes running in this session.
        /// </summary>
        /// <returns>A list of processes.</returns>
        IList<ITerminalServicesProcess> GetProcesses();

        /// <summary>
        ///     Starts remote control of the session.
        /// </summary>
        /// <param name="hotkey">The key to press to stop remote control of the session.</param>
        /// <param name="hotkeyModifiers">The modifiers for the key to press to stop remote control.s</param>
        /// <remarks>
        ///     This method can only be called while running in a remote session. It blocks until remote control
        ///     has ended, which could result from pressing the hotkey, logging off the target session,
        ///     or disconnecting the target session (among other things).
        /// </remarks>
        void StartRemoteControl(ConsoleKey hotkey, RemoteControlHotkeyModifiers hotkeyModifiers);

        /// <summary>
        ///     Stops remote control of the session. The session must be running on the local server.
        /// </summary>
        /// <remarks>
        ///     This method should be called on the session that is being shadowed, not on the session that
        ///     is shadowing.
        /// </remarks>
        void StopRemoteControl();

        /// <summary>
        ///     Connects this session to an existing session. Both sessions must be running on the local server.
        /// </summary>
        /// <param name="target">The session to which to connect.</param>
        /// <param name="password">
        ///     The password of the user logged on to the target session.
        ///     If the user logged on to the target session is the same as the user logged on to this session,
        ///     this parameter can be an empty string.
        /// </param>
        /// <param name="synchronous">
        ///     If <c>true</c>, waits until the operation has completed
        ///     before returning from the method. If <c>false</c>, returns immediately, even though
        ///     the operation may not be complete yet.
        /// </param>
        /// <remarks>
        ///     The user logged on to this session must have permissions to connect to the target session.
        /// </remarks>
        void Connect(ITerminalServicesSession target, string password, bool synchronous);
    }
}