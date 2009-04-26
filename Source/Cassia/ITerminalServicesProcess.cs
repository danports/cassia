using System.Diagnostics;
using System.Security.Principal;

namespace Cassia
{
    /// <summary>
    /// A process running on a terminal server.
    /// </summary>
    public interface ITerminalServicesProcess
    {
        /// <summary>
        /// The ID of the terminal session on the server in which the process is running.
        /// </summary>
        int SessionId { get; }

        /// <summary>
        /// The ID of the process on the server.
        /// </summary>
        int ProcessId { get; }

        /// <summary>
        /// The name of the process, e.g. Notepad.exe.
        /// </summary>
        string ProcessName { get; }

        /// <summary>
        /// The security identifier under which the process is running.
        /// </summary>
        SecurityIdentifier SecurityIdentifier { get; }

        /// <summary>
        /// The terminal server on which this process is running.
        /// </summary>
        ITerminalServer Server { get; }

        /// <summary>
        /// Gets a <see cref="System.Diagnostics.Process" /> object that represents the process.
        /// </summary>
        /// <returns>A <see cref="System.Diagnostics.Process" /> object.</returns>
        Process UnderlyingProcess { get; }

        /// <overloads>
        /// <summary>
        /// Terminates the process.
        /// </summary>
        /// </overloads>
        /// <summary>
        /// Terminates the process immediately.
        /// </summary>
        void Kill();

        /// <summary>
        /// Terminates the process with a particular exit code.
        /// </summary>
        /// <param name="exitCode">The exit code for the process.</param>
        void Kill(int exitCode);
    }
}