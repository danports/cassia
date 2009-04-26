using System.Runtime.CompilerServices;

namespace Cassia
{
    /// <summary>
    /// <para>To use Cassia, construct an instance of <see cref="TerminalServicesManager" />
    /// and then use methods on that class to get information about terminal servers and the sessions
    /// and processes running on them.</para>
    /// <para>Note that if you are connecting to a remote server, you need to
    /// call <see cref="ITerminalServer.Open" /> before accessing server information, but if you are 
    /// connecting to the local server, this is not necessary. For example, to list all sessions 
    /// running on a remote server:
    /// </para>
    /// <code>
    /// ITerminalServicesManager manager = new TerminalServicesManager();
    /// using (ITerminalServer server = manager.GetRemoteServer("ServerName"))
    /// {
    ///     server.Open();
    ///     foreach (ITerminalServicesSession session in server.GetSessions())
    ///     {
    ///         Console.WriteLine("Session " + session.SessionId + ": " + session.UserName);
    ///     }
    /// }
    /// </code>
    /// Another example, this one showing the screen resolution of all clients connected to the local
    /// terminal server:
    /// <code>
    /// ITerminalServicesManager manager = new TerminalServicesManager();
    /// using (ITerminalServer server = manager.GetLocalServer())
    /// {
    ///     // Note that server.Open() is not necessary here since we are accessing the 
    ///     // local terminal server.
    ///     foreach (ITerminalServicesSession session in server.GetSessions())
    ///     {
    ///         IClientDisplay display = session.ClientDisplay;
    ///         if (display != null)
    ///         {
    ///             Console.WriteLine("Session " + session.SessionId + ": " + display.HorizontalResolution + "x"
    ///                               + display.VerticalResolution);
    ///         }
    ///     }
    /// }
    /// </code>
    /// </summary>
    [CompilerGenerated]
    internal class NamespaceDoc {}
}