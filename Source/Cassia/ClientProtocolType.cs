namespace Cassia
{
    /// <summary>
    /// The protocol that a client is using to connect to a terminal server as returned by 
    /// <see cref="ITerminalServicesSession.ClientProtocolType" />.
    /// </summary>
    public enum ClientProtocolType : short
    {
        /// <summary>
        /// The client is directly connected to the console session.
        /// </summary>
        Console = 0,
        /// <summary>
        /// This value exists for legacy purposes.
        /// </summary>
        Legacy = 1,
        /// <summary>
        /// The client is connected via the RDP protocol.
        /// </summary>
        Rdp = 2,
    }
}