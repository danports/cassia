namespace Cassia
{
    public enum ShutdownType
    {
        LogoffAllSessions = 0x00000001,
        Shutdown = 0x00000002,
        Reboot = 0x00000004,
        PowerOff = 0x00000008,
        FastReboot = 0x00000010,
    }
}