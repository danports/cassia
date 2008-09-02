using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Cassia;
using FILETIME=System.Runtime.InteropServices.ComTypes.FILETIME;

namespace Cassia
{
    public class TerminalServicesManager : ITerminalServicesManager
    {
        #region ITerminalServicesManager Members

        public ITerminalServicesSession CurrentSession
        {
            get
            {
                uint returned;
                IntPtr mem;
                if (
                    NativeMethods.WTSQuerySessionInformation(NativeMethods.LocalServerHandle,
                                                             NativeMethods.CurrentSessionId, WTS_INFO_CLASS.WTSSessionId,
                                                             out mem, out returned))
                {
                    uint id = (uint) Marshal.ReadInt32(mem);
                    NativeMethods.WTSFreeMemory(mem);
                    return GetSessionInfo(NativeMethods.LocalServerHandle, id);
                }
                return null;
            }
        }

        public IList<ITerminalServicesSession> GetSessions(string serverName)
        {
            List<ITerminalServicesSession> sessions = new List<ITerminalServicesSession>();
            IntPtr ptrOpenedServer = NativeMethods.LocalServerHandle;

            if (serverName != null)
            {
                ptrOpenedServer = NativeMethods.WTSOpenServer(serverName);
                if (ptrOpenedServer.Equals(IntPtr.Zero))
                {
                    throw new Exception("Terminal Services not running on: " + serverName);
                }
            }
            IntPtr ppSessionInfo;
            uint count;
            if (NativeMethods.WTSEnumerateSessions(ptrOpenedServer, 0, 1, out ppSessionInfo, out count) == 0)
            {
                throw new Exception("Error enumerating sessions: " + Marshal.GetLastWin32Error());
            }
            WTS_SESSION_INFO sessionStruct;
            Int32 current = ppSessionInfo.ToInt32();

            for (int i = 0; i < count; i++)
            {
                try
                {
                    sessionStruct =
                        (WTS_SESSION_INFO) Marshal.PtrToStructure((IntPtr) current, typeof(WTS_SESSION_INFO));
                    current += Marshal.SizeOf(typeof(WTS_SESSION_INFO));
                    sessions.Add(GetSessionInfo(ptrOpenedServer, sessionStruct.SessionID));
                }
                catch (Exception ex)
                {
                    throw new Exception("Failure getting session " + (i + 1) + " of " + count, ex);
                }
            }

            NativeMethods.WTSFreeMemory(ppSessionInfo);
            if (ptrOpenedServer != NativeMethods.LocalServerHandle)
            {
                NativeMethods.WTSCloseServer(ptrOpenedServer);
            }

            return sessions;
        }

        public IList<ITerminalServicesSession> GetSessions()
        {
            return GetSessions(null);
        }

        #endregion

        private static DateTime FileTimeToDateTime(FILETIME ft)
        {
            long hFT = (((long) ft.dwHighDateTime) << 32) + ft.dwLowDateTime;
            return DateTime.FromFileTime(hFT);
        }

        private static TerminalServicesSession GetSessionInfo(IntPtr server, uint sessionId)
        {
            TerminalServicesSession sessionInfo = new TerminalServicesSession();
            sessionInfo.SessionId = sessionId;

            uint returned;
            IntPtr mem;
            if (
                NativeMethods.WTSQuerySessionInformation(server, sessionId, WTS_INFO_CLASS.WTSConnectState, out mem,
                                                         out returned))
            {
                sessionInfo.ConnectionState = (WTS_CONNECTSTATE_CLASS) Marshal.ReadInt32(mem);
                NativeMethods.WTSFreeMemory(mem);
            }

            if (
                NativeMethods.WTSQuerySessionInformation(server, sessionId, WTS_INFO_CLASS.WTSUserName, out mem,
                                                         out returned))
            {
                string str = Marshal.PtrToStringAuto(mem);
                NativeMethods.WTSFreeMemory(mem);
                sessionInfo.UserName = str;
            }

            if (
                NativeMethods.WTSQuerySessionInformation(server, sessionId, WTS_INFO_CLASS.WTSClientName, out mem,
                                                         out returned))
            {
                string str = Marshal.PtrToStringAuto(mem);
                NativeMethods.WTSFreeMemory(mem);
                sessionInfo.ClientName = str;
            }

            GetTimeInfo(server, sessionId, sessionInfo);
            return sessionInfo;
        }

        private static void GetTimeInfo(IntPtr server, uint sessionId, ITerminalServicesSession sessionInfo)
        {
            uint retLen = 0;
            WINSTATIONINFORMATIONW wsInfo = new WINSTATIONINFORMATIONW();
            if (
                NativeMethods.WinStationQueryInformationW(server, sessionId,
                                                          (uint) WINSTATIONINFOCLASS.WinStationInformation, ref wsInfo,
                                                          (uint) Marshal.SizeOf(typeof(WINSTATIONINFORMATIONW)),
                                                          ref retLen) != 0)
            {
                sessionInfo.ConnectTime = FileTimeToDateTime(wsInfo.ConnectTime);
                sessionInfo.CurrentTime = FileTimeToDateTime(wsInfo.CurrentTime);
                sessionInfo.DisconnectTime = FileTimeToDateTime(wsInfo.DisconnectTime);
                sessionInfo.LastInputTime = FileTimeToDateTime(wsInfo.LastInputTime);
                sessionInfo.LoginTime = FileTimeToDateTime(wsInfo.LoginTime);
            }
        }
    }
}