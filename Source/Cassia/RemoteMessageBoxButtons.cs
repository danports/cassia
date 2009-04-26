using System;

namespace Cassia
{
    /// <summary>
    /// Specifies the combination of buttons that should be displayed in a message box shown with the
    /// <see cref="ITerminalServicesSession.MessageBox(string, string, RemoteMessageBoxButtons, RemoteMessageBoxIcon, RemoteMessageBoxDefaultButton, RemoteMessageBoxOptions, TimeSpan, bool)">
    /// ITerminalServicesSession.MessageBox</see> method.
    /// </summary>
    public enum RemoteMessageBoxButtons
    {
        /// <summary>
        /// Show only an "OK" button. This is the default.
        /// </summary>
        Ok = 0,
        /// <summary>
        /// Show "OK" and "Cancel" buttons.
        /// </summary>
        OkCancel = 1,
        /// <summary>
        /// Show "Abort", "Retry", and "Ignore" buttons.
        /// </summary>
        AbortRetryIgnore = 2,
        /// <summary>
        /// Show "Yes", "No", and "Cancel" buttons.
        /// </summary>
        YesNoCancel = 3,
        /// <summary>
        /// Show "Yes" and "No" buttons.
        /// </summary>
        YesNo = 4,
        /// <summary>
        /// Show "Retry" and "Cancel" buttons.
        /// </summary>
        RetryCancel = 5,
    }
}