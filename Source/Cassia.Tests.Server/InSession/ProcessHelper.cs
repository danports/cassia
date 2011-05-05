using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace Cassia.Tests.Server.InSession
{
    public static class ProcessHelper
    {
        #region CreationFlags enum

        [Flags]
        public enum CreationFlags
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

        #region LogonFlags enum

        [Flags]
        public enum LogonFlags
        {
            None = 0,
            LogonWithProfile = 1,
            NetCredentialsOnly = 2,
        }

        #endregion

        #region SecurityImpersonationLevel enum

        public enum SecurityImpersonationLevel
        {
            SecurityAnonymous,
            SecurityIdentification,
            SecurityImpersonation,
            SecurityDelegation
        }

        #endregion

        #region StartupInfoFlags enum

        [Flags]
        public enum StartupInfoFlags
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

        #region TokenInformationClass enum

        public enum TokenInformationClass
        {
            TokenUser = 1,
            TokenGroups,
            TokenPrivileges,
            TokenOwner,
            TokenPrimaryGroup,
            TokenDefaultDacl,
            TokenSource,
            TokenType,
            TokenImpersonationLevel,
            TokenStatistics,
            TokenRestrictedSids,
            TokenSessionId,
            TokenGroupsAndPrivileges,
            TokenSessionReference,
            TokenSandBoxInert,
            TokenAuditPolicy,
            TokenOrigin,
            TokenElevationType,
            TokenLinkedToken,
            TokenElevation,
            TokenHasRestrictions,
            TokenAccessInformation,
            TokenVirtualizationAllowed,
            TokenVirtualizationEnabled,
            TokenIntegrityLevel,
            TokenUIAccess,
            TokenMandatoryPolicy,
            TokenLogonSid,
            MaxTokenInfoClass
        }

        #endregion

        #region TokenType enum

        public enum TokenType
        {
            TokenPrimary = 1,
            TokenImpersonation
        }

        #endregion

        private static bool UserAccountControlExists
        {
            get
            {
                // TODO: there's probably a nicer way to do this.
                return Environment.OSVersion.Version >= new Version(6, 0); // Vista
            }
        }

        [DllImport("Advapi32.dll", SetLastError = true)]
        private static extern int GetTokenInformation(IntPtr token, TokenInformationClass infoClass,
                                                      ref TOKEN_LINKED_TOKEN buffer, int bufferLength,
                                                      out int returnLength);

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

        [DllImport("Advapi32.dll", SetLastError = true)]
        private static extern int DuplicateTokenEx(IntPtr token, int desiredAccess, IntPtr tokenAttributes,
                                                   SecurityImpersonationLevel impersonationLevel, TokenType tokenType,
                                                   out IntPtr newToken);

        public static Process Start(int sessionId, string applicationName, string commandLine)
        {
            var hToken = IntPtr.Zero;
            if (WTSQueryUserToken(sessionId, ref hToken) == 0)
            {
                throw new Win32Exception();
            }

            var hAdminToken = hToken;
            if (UserAccountControlExists)
            {
                var linked = new TOKEN_LINKED_TOKEN();
                int returned;
                if (
                    GetTokenInformation(hToken, TokenInformationClass.TokenLinkedToken, ref linked,
                                        Marshal.SizeOf(linked), out returned) == 0)
                {
                    throw new Win32Exception();
                }

                var hLinkedToken = linked.LinkedToken;
                if (
                    DuplicateTokenEx(hLinkedToken, 0, IntPtr.Zero, SecurityImpersonationLevel.SecurityImpersonation,
                                     TokenType.TokenPrimary, out hAdminToken) == 0)
                {
                    throw new Win32Exception();
                }

                CloseHandle(hLinkedToken);
                CloseHandle(hToken);
            }

            try
            {
                var commandLineText = new StringBuilder(string.Format("\"{0}\" {1}", applicationName, commandLine));
                var info = new StartupInfo();
                info.Cb = Marshal.SizeOf(info);
                ProcessInformation processInfo;

                IntPtr hEnvironment;
                if (CreateEnvironmentBlock(out hEnvironment, hAdminToken, true) == 0)
                {
                    throw new Win32Exception();
                }

                try
                {
                    if (
                        CreateProcessAsUser(hAdminToken, null, commandLineText, IntPtr.Zero, IntPtr.Zero, false,
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
                CloseHandle(hAdminToken);
            }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ProcessInformation
    {
        public readonly IntPtr Process;
        public readonly IntPtr Thread;
        public readonly int ProcessId;
        public readonly int ThreadId;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct StartupInfo
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
        public readonly ProcessHelper.StartupInfoFlags Flags;
        public readonly short ShowWindow;
        public readonly short Reserved2;
        public readonly IntPtr Reserved3;
        public readonly IntPtr StandardInput;
        public readonly IntPtr StandardOutput;
        public readonly IntPtr StandardError;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct TOKEN_LINKED_TOKEN
    {
        public IntPtr LinkedToken;
    }
}