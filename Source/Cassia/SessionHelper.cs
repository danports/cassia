using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using FILETIME=System.Runtime.InteropServices.ComTypes.FILETIME;

namespace Cassia
{
    internal static class SessionHelper
    {
        internal static WTS_CONNECTSTATE_CLASS GetConnectionState(TerminalServerHandle server, uint sessionId)
        {
            uint returned;
            IntPtr mem;
            if (
                NativeMethods.WTSQuerySessionInformation(server.Handle, sessionId, WTS_INFO_CLASS.WTSConnectState,
                                                         out mem, out returned))
            {
                try
                {
                    return (WTS_CONNECTSTATE_CLASS) Marshal.ReadInt32(mem);
                }
                finally
                {
                    NativeMethods.WTSFreeMemory(mem);
                }
            }
            else
            {
                throw new Win32Exception();
            }
        }

        internal static string GetClientName(TerminalServerHandle server, uint sessionId)
        {
            uint returned;
            IntPtr mem;
            if (
                NativeMethods.WTSQuerySessionInformation(server.Handle, sessionId, WTS_INFO_CLASS.WTSClientName, out mem,
                                                         out returned))
            {
                try
                {
                    return Marshal.PtrToStringAuto(mem);
                }
                finally
                {
                    NativeMethods.WTSFreeMemory(mem);
                }
            }
            else
            {
                throw new Win32Exception();
            }
        }

        internal static string GetUserName(TerminalServerHandle server, uint sessionId)
        {
            uint returned;
            IntPtr mem;
            if (
                NativeMethods.WTSQuerySessionInformation(server.Handle, sessionId, WTS_INFO_CLASS.WTSUserName, out mem,
                                                         out returned))
            {
                try
                {
                    return Marshal.PtrToStringAuto(mem);
                }
                finally
                {
                    NativeMethods.WTSFreeMemory(mem);
                }
            }
            else
            {
                throw new Win32Exception();
            }
        }

        internal static WTSINFO GetWtsInfo(TerminalServerHandle server, uint sessionId)
        {
            uint returned;
            IntPtr mem;
            if (
                NativeMethods.WTSQuerySessionInformation(server.Handle, sessionId, WTS_INFO_CLASS.WTSSessionInfo,
                                                         out mem, out returned))
            {
                try
                {
                    return (WTSINFO) Marshal.PtrToStructure(mem, typeof(WTSINFO));
                }
                finally
                {
                    NativeMethods.WTSFreeMemory(mem);
                }
            }
            else
            {
                throw new Win32Exception();
            }
        }

        internal static WINSTATIONINFORMATIONW GetWinStationInformation(TerminalServerHandle server, uint sessionId)
        {
            uint retLen = 0;
            WINSTATIONINFORMATIONW wsInfo = new WINSTATIONINFORMATIONW();
            if (
                NativeMethods.WinStationQueryInformationW(server.Handle, sessionId,
                                                          (uint) WINSTATIONINFOCLASS.WinStationInformation, ref wsInfo,
                                                          (uint) Marshal.SizeOf(typeof(WINSTATIONINFORMATIONW)),
                                                          ref retLen) != 0)
            {
                return wsInfo;
            }
            else
            {
                // Not sure if the function used here sets the error code.
                throw new Win32Exception("Failed to get information for session ID " + sessionId);
            }
        }

        private static DateTime FileTimeToDateTime(FILETIME ft)
        {
            long hFT = (((long) ft.dwHighDateTime) << 32) + ft.dwLowDateTime;
            return DateTime.FromFileTime(hFT);
        }

        internal static TerminalServicesSession GetSessionInfo(TerminalServerHandle server, uint sessionId)
        {
            TerminalServicesSession sessionInfo = new TerminalServicesSession();
            sessionInfo.SessionId = sessionId;
            sessionInfo.ConnectionState = GetConnectionState(server, sessionId);
            sessionInfo.ClientName = GetClientName(server, sessionId);

            if (Environment.OSVersion.Version > new Version(6, 0))
            {
                // We can actually use documented APIs in Vista / Windows Server 2008+.
                WTSINFO info = GetWtsInfo(server, sessionId);
                sessionInfo.ConnectTime = DateTime.FromFileTime(info.ConnectTime);
                sessionInfo.CurrentTime = DateTime.FromFileTime(info.CurrentTime);
                sessionInfo.DisconnectTime = DateTime.FromFileTime(info.DisconnectTime);
                sessionInfo.LastInputTime = DateTime.FromFileTime(info.LastInputTime);
                sessionInfo.LoginTime = DateTime.FromFileTime(info.LogonTime);
                sessionInfo.UserName = info.UserName;
            }
            else
            {
                WINSTATIONINFORMATIONW wsInfo = GetWinStationInformation(server, sessionId);
                sessionInfo.ConnectTime = FileTimeToDateTime(wsInfo.ConnectTime);
                sessionInfo.CurrentTime = FileTimeToDateTime(wsInfo.CurrentTime);
                sessionInfo.DisconnectTime = FileTimeToDateTime(wsInfo.DisconnectTime);
                sessionInfo.LastInputTime = FileTimeToDateTime(wsInfo.LastInputTime);
                sessionInfo.LoginTime = FileTimeToDateTime(wsInfo.LoginTime);
                sessionInfo.UserName = GetUserName(server, sessionId);
            }
            return sessionInfo;
        }

        internal static IList<WTS_SESSION_INFO> GetSessionInfos(TerminalServerHandle server)
        {
            IntPtr ppSessionInfo;
            uint count;

            if (NativeMethods.WTSEnumerateSessions(server.Handle, 0, 1, out ppSessionInfo, out count) == 0)
            {
                throw new Win32Exception();
            }
            try
            {
                List<WTS_SESSION_INFO> results = new List<WTS_SESSION_INFO>();
                Int32 current = ppSessionInfo.ToInt32();
                for (int i = 0; i < count; i++)
                {
                    results.Add((WTS_SESSION_INFO) Marshal.PtrToStructure((IntPtr) current, typeof(WTS_SESSION_INFO)));
                    current += Marshal.SizeOf(typeof(WTS_SESSION_INFO));
                }
                return results;
            }
            finally
            {
                NativeMethods.WTSFreeMemory(ppSessionInfo);
            }
        }

        internal static uint GetCurrentSessionId(TerminalServerHandle server)
        {
            uint returned;
            IntPtr mem;
            if (
                NativeMethods.WTSQuerySessionInformation(server.Handle, NativeMethods.CurrentSessionId,
                                                         WTS_INFO_CLASS.WTSSessionId, out mem, out returned))
            {
                try
                {
                    return (uint) Marshal.ReadInt32(mem);
                }
                finally
                {
                    NativeMethods.WTSFreeMemory(mem);
                }
            }
            else
            {
                throw new Win32Exception();
            }
        }
    }
}