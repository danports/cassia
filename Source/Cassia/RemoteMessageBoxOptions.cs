using System;

namespace Cassia
{
    /// <summary>
    /// Specifies additional options for a message box shown with the
    /// <see cref="ITerminalServicesSession.MessageBox(string, string, RemoteMessageBoxButtons, RemoteMessageBoxIcon, RemoteMessageBoxDefaultButton, RemoteMessageBoxOptions, TimeSpan, bool)">
    /// ITerminalServicesSession.MessageBox</see> method.
    /// </summary>
    [Flags]
    public enum RemoteMessageBoxOptions
    {
        /// <summary>
        /// No additional options. This is the default.
        /// </summary>
        None = 0,
        /// <summary>
        /// Specifies that the text in the message box should be right-aligned. The default is left-aligned.
        /// </summary>
        RightAligned = 0x00080000,
        /// <summary>
        /// Specifies that the message box should use a right-to-left reading order.
        /// </summary>
        RtlReading = 0x00100000,
        /// <summary>
        /// Specifies that the message box should be set to the foreground window when displayed.
        /// </summary>
        SetForeground = 0x00010000,
        /// <summary>
        /// Specifies that the message box should appear above all other windows on the screen.
        /// </summary>
        TopMost = 0x00080000,
    }
}