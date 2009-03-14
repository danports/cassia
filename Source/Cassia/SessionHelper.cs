using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using FILETIME=System.Runtime.InteropServices.ComTypes.FILETIME;

namespace Cassia
{
    internal static class SessionHelper
    {
        public static WTS_CONNECTSTATE_CLASS GetConnectionState(ITerminalServerHandle server, uint sessionId)
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

        public static string GetClientName(ITerminalServerHandle server, uint sessionId)
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

        public static string GetUserName(ITerminalServerHandle server, uint sessionId)
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

        public static WTSINFO GetWtsInfo(ITerminalServerHandle server, uint sessionId)
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

        public static WINSTATIONINFORMATIONW GetWinStationInformation(ITerminalServerHandle server, uint sessionId)
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

        public static TerminalServicesSession GetSessionInfo(ITerminalServer server, ITerminalServerHandle serverHandle,
                                                             uint sessionId)
        {
            TerminalServicesSession sessionInfo = new TerminalServicesSession(server);
            sessionInfo.SessionId = sessionId;
            sessionInfo.ConnectionState = GetConnectionState(serverHandle, sessionId);
            sessionInfo.ClientName = GetClientName(serverHandle, sessionId);

            if (Environment.OSVersion.Version > new Version(6, 0))
            {
                // We can actually use documented APIs in Vista / Windows Server 2008+.
                WTSINFO info = GetWtsInfo(serverHandle, sessionId);
                sessionInfo.ConnectTime = DateTime.FromFileTime(info.ConnectTime);
                sessionInfo.CurrentTime = DateTime.FromFileTime(info.CurrentTime);
                sessionInfo.DisconnectTime = DateTime.FromFileTime(info.DisconnectTime);
                sessionInfo.LastInputTime = DateTime.FromFileTime(info.LastInputTime);
                sessionInfo.LoginTime = DateTime.FromFileTime(info.LogonTime);
                sessionInfo.UserName = info.UserName;
            }
            else
            {
                WINSTATIONINFORMATIONW wsInfo = GetWinStationInformation(serverHandle, sessionId);
                sessionInfo.ConnectTime = FileTimeToDateTime(wsInfo.ConnectTime);
                sessionInfo.CurrentTime = FileTimeToDateTime(wsInfo.CurrentTime);
                sessionInfo.DisconnectTime = FileTimeToDateTime(wsInfo.DisconnectTime);
                sessionInfo.LastInputTime = FileTimeToDateTime(wsInfo.LastInputTime);
                sessionInfo.LoginTime = FileTimeToDateTime(wsInfo.LoginTime);
                sessionInfo.UserName = GetUserName(serverHandle, sessionId);
            }
            return sessionInfo;
        }

        public static IList<WTS_SESSION_INFO> GetSessionInfos(ITerminalServerHandle server)
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
                long pointer = ppSessionInfo.ToInt64();
                for (int i = 0; i < count; i++)
                {
                    results.Add((WTS_SESSION_INFO) Marshal.PtrToStructure(new IntPtr(pointer), typeof(WTS_SESSION_INFO)));
                    pointer += Marshal.SizeOf(typeof(WTS_SESSION_INFO));
                }
                return results;
            }
            finally
            {
                NativeMethods.WTSFreeMemory(ppSessionInfo);
            }
        }

        public static uint GetCurrentSessionId(ITerminalServerHandle server)
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

        public static void LogoffSession(ITerminalServerHandle server, uint sessionId, bool wait)
        {
            if (NativeMethods.WTSLogoffSession(server.Handle, sessionId, wait) == 0)
            {
                throw new Win32Exception();
            }
        }

        public static void DisconnectSession(ITerminalServerHandle server, uint sessionId, bool wait)
        {
            if (NativeMethods.WTSDisconnectSession(server.Handle, sessionId, wait) == 0)
            {
                throw new Win32Exception();
            }
        }

        public static RemoteMessageBoxResult SendMessage(ITerminalServerHandle server, uint sessionId, string title,
                                                         string message, uint style, uint timeout, bool wait)
        {
            RemoteMessageBoxResult result;
            title = title ?? string.Empty;
            message = message ?? string.Empty;
            if (
                NativeMethods.WTSSendMessage(server.Handle, sessionId, title,
                                             (uint) (title.Length * Marshal.SystemDefaultCharSize), message,
                                             (uint) (message.Length * Marshal.SystemDefaultCharSize), style, timeout,
                                             out result, wait) == 0)
            {
                throw new Win32Exception();
            }
            return result;
        }

        public static IList<string> EnumerateServers(string domainName)
        {
            IntPtr ppServerInfo;
            uint count;
            if (NativeMethods.WTSEnumerateServers(domainName, 0, 1, out ppServerInfo, out count) == 0)
            {
                throw new Win32Exception();
            }
            try
            {
                List<string> result = new List<string>();
                long pointer = ppServerInfo.ToInt64();
                for (uint index = 0; index < count; index++)
                {
                    WTS_SERVER_INFO info =
                        (WTS_SERVER_INFO) Marshal.PtrToStructure(new IntPtr(pointer), typeof(WTS_SERVER_INFO));
                    result.Add(info.ServerName);
                    pointer += Marshal.SizeOf(typeof(WTS_SERVER_INFO));
                }
                return result;
            }
            finally
            {
                NativeMethods.WTSFreeMemory(ppServerInfo);
            }
        }
    }
}