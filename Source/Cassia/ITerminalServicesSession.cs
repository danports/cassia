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
        long SessionId { get; }
        string UserName { get; }
        void Logoff();
        void Logoff(bool synchronous);
        void Disconnect();
        void Disconnect(bool synchronous);
    }
}