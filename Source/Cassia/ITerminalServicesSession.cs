using System;
using Cassia;

namespace Cassia
{
    public interface ITerminalServicesSession
    {
        string ClientName { get; set; }
        WTS_CONNECTSTATE_CLASS ConnectionState { get; set; }
        DateTime ConnectTime { get; set; }
        DateTime CurrentTime { get; set; }
        DateTime DisconnectTime { get; set; }
        DateTime LastInputTime { get; set; }
        DateTime LoginTime { get; set; }
        long SessionId { get; set; }
        string UserName { get; set; }
    }
}