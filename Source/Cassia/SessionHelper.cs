using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using FILETIME=System.Runtime.InteropServices.ComTypes.FILETIME;

namespace Cassia
{
    internal static class SessionHelper
    {
        #region Delegates

        public delegate void ListProcessInfosCallback(WTS_PROCESS_INFO processInfo);

        #endregion

        public static WTS_CONNECTSTATE_CLASS GetConnectionState(ITerminalServerHandle server, int sessionId)
        {
            int returned;
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

        public static string GetClientName(ITerminalServerHandle server, int sessionId)
        {
            int returned;
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

        public static string GetUserName(ITerminalServerHandle server, int sessionId)
        {
            int returned;
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

        public static WTSINFO GetWtsInfo(ITerminalServerHandle server, int sessionId)
        {
            int returned;
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

        public static WINSTATIONINFORMATIONW GetWinStationInformation(ITerminalServerHandle server, int sessionId)
        {
            int retLen = 0;
            WINSTATIONINFORMATIONW wsInfo = new WINSTATIONINFORMATIONW();
            if (
                NativeMethods.WinStationQueryInformationW(server.Handle, sessionId,
                                                          (int) WINSTATIONINFOCLASS.WinStationInformation, ref wsInfo,
                                                          Marshal.SizeOf(typeof(WINSTATIONINFORMATIONW)), ref retLen)
                != 0)
            {
                return wsInfo;
            }
            else
            {
                // Not sure if the function used here sets the error code.
                throw new Win32Exception("Failed to get information for session ID " + sessionId);
            }
        }

        public static DateTime FileTimeToDateTime(FILETIME ft)
        {
            long hFT = (((long) ft.dwHighDateTime) << 32) + ft.dwLowDateTime;
            return DateTime.FromFileTime(hFT);
        }

        public static IList<WTS_SESSION_INFO> GetSessionInfos(ITerminalServerHandle server)
        {
            IntPtr ppSessionInfo;
            int count;

            if (NativeMethods.WTSEnumerateSessions(server.Handle, 0, 1, out ppSessionInfo, out count) == 0)
            {
                throw new Win32Exception();
            }
            try
            {
                return PtrToStructureList<WTS_SESSION_INFO>(ppSessionInfo, count);
            }
            finally
            {
                NativeMethods.WTSFreeMemory(ppSessionInfo);
            }
        }

        public static int GetCurrentSessionId(ITerminalServerHandle server)
        {
            int returned;
            IntPtr mem;
            if (
                NativeMethods.WTSQuerySessionInformation(server.Handle, NativeMethods.CurrentSessionId,
                                                         WTS_INFO_CLASS.WTSSessionId, out mem, out returned))
            {
                try
                {
                    return Marshal.ReadInt32(mem);
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

        public static void LogoffSession(ITerminalServerHandle server, int sessionId, bool wait)
        {
            if (NativeMethods.WTSLogoffSession(server.Handle, sessionId, wait) == 0)
            {
                throw new Win32Exception();
            }
        }

        public static void DisconnectSession(ITerminalServerHandle server, int sessionId, bool wait)
        {
            if (NativeMethods.WTSDisconnectSession(server.Handle, sessionId, wait) == 0)
            {
                throw new Win32Exception();
            }
        }

        public static RemoteMessageBoxResult SendMessage(ITerminalServerHandle server, int sessionId, string title,
                                                         string message, int style, int timeout, bool wait)
        {
            RemoteMessageBoxResult result;
            title = title ?? string.Empty;
            message = message ?? string.Empty;
            if (
                NativeMethods.WTSSendMessage(server.Handle, sessionId, title,
                                             title.Length * Marshal.SystemDefaultCharSize, message,
                                             message.Length * Marshal.SystemDefaultCharSize, style, timeout, out result,
                                             wait) == 0)
            {
                throw new Win32Exception();
            }
            return result;
        }

        public static IList<WTS_SERVER_INFO> EnumerateServers(string domainName)
        {
            IntPtr ppServerInfo;
            int count;
            if (NativeMethods.WTSEnumerateServers(domainName, 0, 1, out ppServerInfo, out count) == 0)
            {
                throw new Win32Exception();
            }
            try
            {
                return PtrToStructureList<WTS_SERVER_INFO>(ppServerInfo, count);
            }
            finally
            {
                NativeMethods.WTSFreeMemory(ppServerInfo);
            }
        }

        private static IList<T> PtrToStructureList<T>(IntPtr ppList, int count) where T : struct
        {
            List<T> result = new List<T>();
            long pointer = ppList.ToInt64();
            int sizeOf = Marshal.SizeOf(typeof(T));
            for (int index = 0; index < count; index++)
            {
                T item = (T) Marshal.PtrToStructure(new IntPtr(pointer), typeof(T));
                result.Add(item);
                pointer += sizeOf;
            }
            return result;
        }

        public static void ForEachProcessInfo(ITerminalServerHandle server, ListProcessInfosCallback callback)
        {
            IntPtr ppProcessInfo;
            int count;
            if (NativeMethods.WTSEnumerateProcesses(server.Handle, 0, 1, out ppProcessInfo, out count) == 0)
            {
                throw new Win32Exception();
            }
            try
            {
                // We can't just return a list of WTS_PROCESS_INFOs because those have pointers to 
                // SIDs that have to be copied into managed memory first. So we use a callback instead.
                IList<WTS_PROCESS_INFO> processInfos = PtrToStructureList<WTS_PROCESS_INFO>(ppProcessInfo, count);
                foreach (WTS_PROCESS_INFO processInfo in processInfos)
                {
                    // It seems that WTSEnumerateProcesses likes to return an empty struct in the first 
                    // element of the array, so we ignore that here.
                    // TODO: Find out why this happens.
                    if (processInfo.ProcessId != 0)
                    {
                        callback(processInfo);
                    }
                }
            }
            finally
            {
                NativeMethods.WTSFreeMemory(ppProcessInfo);
            }
        }
    }
}