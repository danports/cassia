namespace Cassia
{
    public enum RemoteMessageBoxIcon
    {
        Hand = 0x10,
        Question = 0x20,
        Exclamation = 0x30,
        Asterisk = 0x40,
        Warning = Exclamation,
        Error = Hand,
        Information = Asterisk,
        Stop = Hand,
    }
}