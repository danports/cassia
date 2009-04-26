using System.Runtime.CompilerServices;

namespace Cassia
{
    /// <summary>
    /// To use Cassia, you should construct an instance of <see cref="TerminalServicesManager" />
    /// and then use methods on that class to get information about terminal servers and the sessions
    /// and processes running on them. Note that if you are connecting to a remote server, you need to
    /// call <see cref="ITerminalServer.Open" /> before accessing server information, but if you are 
    /// connecting to the local server, this is not necessary. For example, to list all sessions 
    /// running on a remote server:
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
    /// </summary>
    [CompilerGenerated]
    internal class NamespaceDoc {}
}