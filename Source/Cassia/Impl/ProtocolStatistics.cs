namespace Cassia.Impl
{
    public class ProtocolStatistics : IProtocolStatistics
    {
        private readonly int _bytes;
        private readonly int _compressedBytes;
        private readonly int _frames;

        public ProtocolStatistics(int bytes, int frames, int compressedBytes)
        {
            _bytes = bytes;
            _frames = frames;
            _compressedBytes = compressedBytes;
        }

        public ProtocolStatistics(PROTOCOLCOUNTERS counters)
        {
            _bytes = counters.Bytes;
            _frames = counters.Frames;
            _compressedBytes = counters.CompressedBytes;
        }

        #region IProtocolStatistics Members

        public int Bytes
        {
            get { return _bytes; }
        }

        public int Frames
        {
            get { return _frames; }
        }

        public int CompressedBytes
        {
            get { return _compressedBytes; }
        }

        #endregion
    }
}