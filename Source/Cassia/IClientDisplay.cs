namespace Cassia
{
    /// <summary>
    /// Information about a remote client's display.
    /// </summary>
    public interface IClientDisplay
    {
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
    }
}