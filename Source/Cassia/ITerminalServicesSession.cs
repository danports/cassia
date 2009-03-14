using System;

namespace Cassia
{
    public interface ITerminalServicesSession
    {
        string ClientName { get; }
        WTS_CONNECTSTATE_CLASS ConnectionState { get; }
        DateTime ConnectTime { get; }
        DateTime CurrentTime { get; }
        DateTime DisconnectTime { get; }
        DateTime LastInputTime { get; }
        DateTime LoginTime { get; }
        int SessionId { get; }
        string UserName { get; }
        void Logoff();
        void Logoff(bool synchronous);
        void Disconnect();
        void Disconnect(bool synchronous);
        void MessageBox(string text);
        void MessageBox(string text, string caption);
        void MessageBox(string text, string caption, RemoteMessageBoxIcon icon);

        RemoteMessageBoxResult MessageBox(string text, string caption, RemoteMessageBoxButtons buttons,
                                                 RemoteMessageBoxIcon icon, RemoteMessageBoxDefaultButton defaultButton,
                                                 RemoteMessageBoxOptions options, TimeSpan timeout, bool synchronous);
    }
}