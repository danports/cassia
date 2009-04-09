using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Principal;

namespace Cassia
{
    /// <summary>
    /// A session on a terminal server.
    /// </summary>
    public interface ITerminalServicesSession
    {
        /// <summary>
        /// The name of the machine last connected to this session.
        /// </summary>
        string ClientName { get; }

        /// <summary>
        /// The connection state of the session.
        /// </summary>
        WTS_CONNECTSTATE_CLASS ConnectionState { get; }

        /// <summary>
        /// The time at which the user connected to this session.
        /// </summary>
        DateTime ConnectTime { get; }

        /// <summary>
        /// The current time in the session.
        /// </summary>
        DateTime CurrentTime { get; }

        /// <summary>
        /// The time at which the user disconnected from this session.
        /// </summary>
        DateTime DisconnectTime { get; }

        /// <summary>
        /// The time at which this session last received input -- mouse movements, key presses, etc.
        /// </summary>
        DateTime LastInputTime { get; }

        /// <summary>
        /// The time at which the user logged into this session for the first time.
        /// </summary>
        DateTime LoginTime { get; }

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
        NTAccount Account { get; }

        /// <summary>
        /// The number of bits used per pixel in the client's connection to the session.
        /// </summary>
        int BitsPerPixel { get; }

        /// <summary>
        /// The horizontal resolution of the client's display.
        /// </summary>
        /// <remarks>This may not be the same as the horizontal resolution of the client's monitor -- 
        /// it only reflects the size of the RDP connection window on the client.</remarks>
        int HorizontalResolution { get; }

        /// <summary>
        /// The vertical resolution of the client's display.
        /// </summary>
        /// <remarks>This may not be the same as the vertical resolution of the client's monitor -- 
        /// it only reflects the size of the RDP connection window on the client.</remarks>
        int VerticalResolution { get; }

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
        IPAddress IPAddress { get; }

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
        /// before closing the message box. This will be treated as a integer number of seconds --
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