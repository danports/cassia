using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace Cassia.Tests.Server.InSession
{
    public static class ProcessHelper
    {
        [DllImport("Wtsapi32.dll", SetLastError = true)]
        private static extern int WTSQueryUserToken(int sessionId, ref IntPtr token);

        [DllImport("Advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern int CreateProcessAsUser(IntPtr token, string applicationName, StringBuilder commandLine,
                                                      IntPtr processAttributes, IntPtr threadAttributes,
                                                      bool inheritHandles, CreationFlags creationFlags,
                                                      IntPtr environment, string currentDirectory,
                                                      ref StartupInfo startupInfo,
                                                      out ProcessInformation processInformation);

        [DllImport("Kernel32.dll", SetLastError = true)]
        private static extern int CloseHandle(IntPtr handle);

        [DllImport("Userenv.dll", SetLastError = true)]
        private static extern int CreateEnvironmentBlock(out IntPtr environment, IntPtr token, bool inherit);

        [DllImport("Userenv.dll", SetLastError = true)]
        private static extern int DestroyEnvironmentBlock(IntPtr environment);

        public static Process Start(int sessionId, string applicationName, string commandLine)
        {
            var hToken = IntPtr.Zero;
            if (WTSQueryUserToken(sessionId, ref hToken) == 0)
            {
                throw new Win32Exception();
            }

            try
            {
                var commandLineText = new StringBuilder(string.Format("\"{0}\" {1}", applicationName, commandLine));
                var info = new StartupInfo();
                info.Cb = Marshal.SizeOf(info);
                ProcessInformation processInfo;

                IntPtr hEnvironment;
                if (CreateEnvironmentBlock(out hEnvironment, hToken, true) == 0)
                {
                    throw new Win32Exception();
                }

                try
                {
                    if (
                        CreateProcessAsUser(hToken, null, commandLineText, IntPtr.Zero, IntPtr.Zero, false,
                                            CreationFlags.UnicodeEnvironment, hEnvironment, null, ref info,
                                            out processInfo) == 0)
                    {
                        throw new Win32Exception();
                    }
                }
                finally
                {
                    DestroyEnvironmentBlock(hEnvironment);
                }

                CloseHandle(processInfo.Process);
                CloseHandle(processInfo.Thread);
                return Process.GetProcessById(processInfo.ProcessId);
            }
            finally
            {
                CloseHandle(hToken);
            }
        }

        #region Nested type: CreationFlags

        [Flags]
        private enum CreationFlags
        {
            Default = DefaultErrorMode | NewConsole | NewProcessGroup,
            DefaultErrorMode = 0x4000000,
            NewConsole = 0x10,
            NewProcessGroup = 0x200,
            SeparateWowVdm = 0x800,
            Suspended = 0x4,
            UnicodeEnvironment = 0x400,
            ExtendedStartupInfoPresent = 0x80000,
        }

        #endregion

        #region Nested type: LogonFlags

        [Flags]
        private enum LogonFlags
        {
            None = 0,
            LogonWithProfile = 1,
            NetCredentialsOnly = 2,
        }

        #endregion

        #region Nested type: ProcessInformation

        [StructLayout(LayoutKind.Sequential)]
        private struct ProcessInformation
        {
            public readonly IntPtr Process;
            public readonly IntPtr Thread;
            public readonly int ProcessId;
            public readonly int ThreadId;
        }

        #endregion

        #region Nested type: StartupInfo

        [StructLayout(LayoutKind.Sequential)]
        private struct StartupInfo
        {
            public int Cb;
            public readonly string Reserved;
            public readonly string Desktop;
            public readonly string Title;
            public readonly int X;
            public readonly int Y;
            public readonly int XSize;
            public readonly int YSize;
            public readonly int XCountChars;
            public readonly int YCountChars;
            public readonly int FillAttribute;
            public readonly StartupInfoFlags Flags;
            public readonly short ShowWindow;
            public readonly short Reserved2;
            public readonly IntPtr Reserved3;
            public readonly IntPtr StandardInput;
            public readonly IntPtr StandardOutput;
            public readonly IntPtr StandardError;
        }

        #endregion

        #region Nested type: StartupInfoFlags

        [Flags]
        private enum StartupInfoFlags
        {
            ForceOnFeedback = 0x40,
            ForceOffFeedback = 0x80,
            PreventPinning = 0x2000,
            RunFullScreen = 0x20,
            TitleIsAppId = 0x1000,
            TitleIsLinkName = 0x800,
            UseCountChars = 0x8,
            UseFillAttribute = 0x10,
            UseHotKey = 0x200,
            UsePosition = 0x4,
            UseShowWindow = 0x1,
            UseSize = 0x2,
            UseStdHandles = 0x100,
        }

        #endregion
    }
}