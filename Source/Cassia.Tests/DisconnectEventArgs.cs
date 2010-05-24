using System;

namespace Cassia.Tests
{
    public class DisconnectEventArgs : EventArgs
    {
        private readonly int _reason;

        public DisconnectEventArgs(int reason)
        {
            _reason = reason;
        }

        public int Reason
        {
            get { return _reason; }
        }
    }
}