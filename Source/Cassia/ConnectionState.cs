namespace Cassia
{
    /// <summary>
    /// Connection state of a session.
    /// </summary>
    public enum ConnectionState
    {
        /// <summary>
        /// A user is logged on to the session.
        /// </summary>
        Active,
        /// <summary>
        /// A client is connected to the session.
        /// </summary>
        Connected,
        /// <summary>
        /// The session is in the process of connecting to a client.
        /// </summary>
        ConnectQuery,
        /// <summary>
        /// This session is shadowing another session.
        /// </summary>
        Shadowing,
        /// <summary>
        /// The session is active, but the client has disconnected from it.
        /// </summary>
        Disconnected,
        /// <summary>
        /// The session is waiting for a client to connect.
        /// </summary>
        Idle,
        /// <summary>
        /// The session is listening for connections.
        /// </summary>
        Listening,
        /// <summary>
        /// The session is being reset.
        /// </summary>
        Reset,
        /// <summary>
        /// The session is down due to an error.
        /// </summary>
        Down,
        /// <summary>
        /// The session is initializing.
        /// </summary>
        Initializing
    }
}