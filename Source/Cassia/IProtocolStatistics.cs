namespace Cassia
{
    /// <summary>
    /// Contains RDP protocol statistics.
    /// </summary>
    public interface IProtocolStatistics
    {
        /// <summary>
        /// Number of bytes transferred.
        /// </summary>
        int Bytes { get; }

        /// <summary>
        /// Number of frames transferred.
        /// </summary>
        int Frames { get; }

        /// <summary>
        /// Number of compressed bytes transferred.
        /// </summary>
        int CompressedBytes { get; }
    }
}