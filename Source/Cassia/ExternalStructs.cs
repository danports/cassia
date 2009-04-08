using System;
using System.Runtime.InteropServices;
using FILETIME=System.Runtime.InteropServices.ComTypes.FILETIME;

namespace Cassia
{
    // DO NOT RESHARPER THIS FILE. R# will reorder members of unmanaged structs,
    // resulting in access violations at runtime.

    [StructLayout(LayoutKind.Sequential)]
    public struct WTS_CLIENT_ADDRESS
    {
        public int AddressFamily;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
        public byte[] Address;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct WTS_CLIENT_DISPLAY
    {
        public int HorizontalResolution;
        public int VerticalResolution;
        public int ColorDepth;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct WTS_PROCESS_INFO
    {
        public int SessionId;
        public int ProcessId;
        [MarshalAs(UnmanagedType.LPTStr)]
        public string ProcessName;
        public IntPtr UserSid;
    }

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
        WTSOutgoingFrames,
        WTSSessionInfo = 24
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
    internal struct WINSTATIONINFORMATIONW
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 70)]
        private byte[] Reserved1;
        public int SessionId;
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
        public int SessionID;
        [MarshalAs(UnmanagedType.LPTStr)]
        public string WinStationName;
        public WTS_CONNECTSTATE_CLASS State;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct WTS_SERVER_INFO
    {
        [MarshalAs(UnmanagedType.LPTStr)]
        public string ServerName;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    internal struct WTSINFO
    {
        public WTS_CONNECTSTATE_CLASS State;
        public int SessionId;
        public int IncomingBytes;
        public int OutgoingBytes;
        public int IncomingFrames;
        public int OutgoingFrames;
        public int IncomingCompressedBytes;
        public int OutgoingCompressedBytes;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string WinStationName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 17)]
        public string Domain;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 21)]
        public string UserName;
        [MarshalAs(UnmanagedType.I8)]
        public long ConnectTime;
        [MarshalAs(UnmanagedType.I8)]
        public long DisconnectTime;
        [MarshalAs(UnmanagedType.I8)]
        public long LastInputTime;
        [MarshalAs(UnmanagedType.I8)]
        public long LogonTime;
        [MarshalAs(UnmanagedType.I8)]
        public long CurrentTime;
    }
}
