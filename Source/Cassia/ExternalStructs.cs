using System;
using System.Runtime.InteropServices;
using FILETIME=System.Runtime.InteropServices.ComTypes.FILETIME;

namespace Cassia
{
    // DO NOT RESHARPER THIS FILE. R# will reorder members of unmanaged structs,
    // resulting in access violations at runtime.

    public enum WTS_INFO_CLASS
    {
        WTSInitialProgram,
        WTSApplicationName,
        WTSWorkingDirectory,
        WTSOEMId,
        WTSSessionId,
        WTSUserName,
        WTSWinStationName,
        WTSDomainName,
        WTSConnectState,
        WTSClientBuildNumber,
        WTSClientName,
        WTSClientDirectory,
        WTSClientProductId,
        WTSClientHardwareId,
        WTSClientAddress,
        WTSClientDisplay,
        WTSClientProtocolType,
        WTSIdleTime,
        WTSLogonTime,
        WTSIncomingBytes,
        WTSOutgoingBytes,
        WTSIncomingFrames,
        WTSOutgoingFrames
    }

    public enum WTS_CONNECTSTATE_CLASS
    {
        WTSActive,
        WTSConnected,
        WTSConnectQuery,
        WTSShadow,
        WTSDisconnected,
        WTSIdle,
        WTSListen,
        WTSReset,
        WTSDown,
        WTSInit
    }

    internal enum WINSTATIONINFOCLASS
    {
        WinStationInformation = 8
    }

    [StructLayout(LayoutKind.Sequential)]
    [CLSCompliant(false)]
    public struct WINSTATIONINFORMATIONW
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 70)]
        private byte[] Reserved1;
        public UInt32 SessionId;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        private byte[] Reserved2;
        public FILETIME ConnectTime;
        public FILETIME DisconnectTime;
        public FILETIME LastInputTime;
        public FILETIME LoginTime;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1096)]
        private byte[] Reserved3;
        public FILETIME CurrentTime;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct WTS_SESSION_INFO
    {
        public uint SessionID;
        [MarshalAs(UnmanagedType.LPTStr)]
        public string WinStationName;
        public WTS_CONNECTSTATE_CLASS State;
    }
}
