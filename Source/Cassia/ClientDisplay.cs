namespace Cassia
{
    /// <summary>
    /// Default implementation of <see cref="IClientDisplay" />.
    /// </summary>
    public class ClientDisplay : IClientDisplay
    {
        private readonly int _bitsPerPixel;
        private readonly int _horizontalResolution;
        private readonly int _verticalResolution;

        public ClientDisplay(WTS_CLIENT_DISPLAY clientDisplay)
        {
            _horizontalResolution = clientDisplay.HorizontalResolution;
            _verticalResolution = clientDisplay.VerticalResolution;
            _bitsPerPixel = GetBitsPerPixel(clientDisplay.ColorDepth);
        }

        #region IClientDisplay Members

        public int BitsPerPixel
        {
            get { return _bitsPerPixel; }
        }

        public int HorizontalResolution
        {
            get { return _horizontalResolution; }
        }

        public int VerticalResolution
        {
            get { return _verticalResolution; }
        }

        #endregion

        private static int GetBitsPerPixel(int colorDepth)
        {
            switch (colorDepth)
            {
                case 1:
                    return 4;
                case 2:
                    return 8;
                case 4:
                    return 16;
                case 8:
                    return 24;
                case 16:
                    return 15;
            }
            return 0;
        }
    }
}