using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Cassia.Impl;
using FILETIME=System.Runtime.InteropServices.ComTypes.FILETIME;

namespace Cassia.Impl
{
    internal static class NativeMethodsHelper
    {
        #region Delegates

        public delegate void ListProcessInfosCallback(WTS_PROCESS_INFO processInfo);

        #endregion

        public static WTS_CONNECTSTATE_CLASS GetConnectionState(ITerminalServerHandle server, int sessionId)
        {
            ProcessSessionCallback<WTS_CONNECTSTATE_CLASS> callback =
                delegate(IntPtr mem, int returned) { return (WTS_CONNECTSTATE_CLASS) Marshal.ReadInt32(mem); };
            return QuerySessionInformation(server, sessionId, WTS_INFO_CLASS.WTSConnectState, callback);
        }

        private static T QuerySessionInformation<T>(ITerminalServerHandle server, int sessionId,
                                                    WTS_INFO_CLASS infoClass, ProcessSessionCallback<T> callback)
        {
            int returned;
            IntPtr mem;
            if (NativeMethods.WTSQuerySessionInformation(server.Handle, sessionId, infoClass, out mem, out returned))
            {
                try
                {
                    return callback(mem, returned);
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

        public static string QuerySessionInformationForString(ITerminalServerHandle server, int sessionId,
                                                              WTS_INFO_CLASS infoClass)
        {
            ProcessSessionCallback<string> callback =
                delegate(IntPtr mem, int returned) { return Marshal.PtrToStringAuto(mem); };
            return QuerySessionInformation(server, sessionId, infoClass, callback);
        }

        public static T QuerySessionInformationForStruct<T>(ITerminalServerHandle server, int sessionId,
                                                            WTS_INFO_CLASS infoClass) where T : struct
        {
            ProcessSessionCallback<T> callback =
                delegate(IntPtr mem, int returned) { return (T) Marshal.PtrToStructure(mem, typeof(T)); };
            return QuerySessionInformation(server, sessionId, infoClass, callback);
        }

        public static WINSTATIONINFORMATIONW GetWinStationInformation(ITerminalServerHandle server, int sessionId)
        {
            int retLen = 0;
            WINSTATIONINFORMATIONW wsInfo = new WINSTATIONINFORMATIONW();
            if (
                NativeMethods.WinStationQueryInformation(server.Handle, sessionId,
                                                         (int) WINSTATIONINFOCLASS.WinStationInformation, ref wsInfo,
                                                         Marshal.SizeOf(typeof(WINSTATIONINFORMATIONW)), ref retLen)
                != 0)
            {
                return wsInfo;
            }
            else
            {
                throw new Win32Exception();
            }
        }

        public static DateTime? FileTimeToDateTime(FILETIME ft)
        {
            if (ft.dwHighDateTime == 0 && ft.dwLowDateTime == 0)
            {
                return null;
            }
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

        public static void TerminateProcess(ITerminalServerHandle server, int processId, int exitCode)
        {
            if (NativeMethods.WTSTerminateProcess(server.Handle, processId, exitCode) == 0)
            {
                throw new Win32Exception();
            }
        }

        public static int QuerySessionInformationForClientBuildNumber(ITerminalServerHandle server, int sessionId)
        {
            ProcessSessionCallback<int> callback = delegate(IntPtr mem, int returned) { return Marshal.ReadInt32(mem); };
            return QuerySessionInformation(server, sessionId, WTS_INFO_CLASS.WTSClientBuildNumber, callback);
        }

        public static void ShutdownSystem(ITerminalServerHandle server, int flags)
        {
            if (NativeMethods.WTSShutdownSystem(server.Handle, flags) == 0)
            {
                throw new Win32Exception();
            }
        }

        #region Nested type: ProcessSessionCallback

        private delegate T ProcessSessionCallback<T>(IntPtr mem, int returnedBytes);

        #endregion
    }
}