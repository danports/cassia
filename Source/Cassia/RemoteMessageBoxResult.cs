namespace Cassia
{
    public enum RemoteMessageBoxResult
    {
        Ok = 1,
        Cancel = 2,
        Abort = 3,
        Retry = 4,
        Ignore = 5,
        Yes = 6,
        No = 7,
        Timeout = 0x7D00,
        Asynchronous = 0x7D01,
    }
}