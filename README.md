# Cassia #
Cassia is a .NET library for accessing the native Windows Remote Desktop Services API (formerly the Terminal Services API).

## What can I do with it? ##
Cassia supports the following operations on both local and remote terminal servers:
  * Enumerating terminal sessions and reporting session information including connection state, user name, client name, client display details, client-reported IP address, and client build number (WTSEnumerateSessions, WTSQuerySessionInformation, and friends)
  * Logging off a session (WTSLogoffSession)
  * Disconnecting a session (WTSDisconnectSession)
  * Displaying a message box in a session and getting a response from the user (WTSSendMessage)
  * Enumerating all processes (WTSEnumerateProcesses)
  * Killing a process (WTSTerminateProcess)
  * Shutting down or rebooting the server (WTSShutdownSystem)
In addition, Cassia supports enumerating all terminal servers on a domain (WTSEnumerateServers).

## Well, that sounds like a blast. So, how do I use it? ##
First [download the release](https://github.com/danports/cassia/releases), unzip it, and add a reference to Cassia (in the Bin folder), or add a reference to the [Cassia NuGet package](https://www.nuget.org/packages/Cassia). Then you can immediately start writing cheesy Hello world applications like this one:
```
ITerminalServicesManager manager = new TerminalServicesManager();
using (ITerminalServer server = manager.GetLocalServer())
{
    server.Open();
    foreach (ITerminalServicesSession session in server.GetSessions())
    {
        Console.WriteLine("Hi there, " + session.UserAccount + " on session " + session.SessionId);
        Console.WriteLine("It looks like you logged on at " + session.LoginTime +
                          " and are now " + session.ConnectionState);
    }
}
```

A sample console application demonstrating all of Cassia's functionality is included in the release (in the Samples\SessionInfo folder).

## What platforms are supported? ##
Cassia is supported on Windows 7, 8, and 10, and on Windows Server 2008, 2008 R2, 2012, 2012 R2, and 2016. Most of the functions will still work on older versions of Windows that are no longer officially supported (Windows XP, Windows Vista, Windows Server 2003, etc.).

## Questions? ##
First of all, you'll want to consult the documentation, located in the Doc folder of the release. If that doesn't do it for you, post to the [mailing list](mailto:cassia-users@googlegroups.com) or post a question on Stack Overflow tagged with [cassia](https://stackoverflow.com/questions/tagged/cassia). Bugs and feature requests can be submitted to the [issue tracker](https://github.com/danports/cassia/issues).