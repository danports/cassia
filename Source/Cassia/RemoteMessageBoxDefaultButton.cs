using System;

namespace Cassia
{
    /// <summary>
    /// Specifies the buttons that should be selected by default in a message box shown with the
    /// <see cref="ITerminalServicesSession.MessageBox(string, string, RemoteMessageBoxButtons, RemoteMessageBoxIcon, RemoteMessageBoxDefaultButton, RemoteMessageBoxOptions, TimeSpan, bool)">
    /// ITerminalServicesSession.MessageBox</see> method.
    /// </summary>
    public enum RemoteMessageBoxDefaultButton
    {
        /// <summary>
        /// The first button should be selected. This is the default.
        /// </summary>
        Button1 = 0,
        /// <summary>
        /// The second button should be selected.
        /// </summary>
        Button2 = 0x100,
        /// <summary>
        /// The third button should be selected.
        /// </summary>
        Button3 = 0x200,
        /// <summary>
        /// The fourth button should be selected.
        /// </summary>
        Button4 = 0x300,
    }
}