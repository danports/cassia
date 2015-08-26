Cassia is a .NET library for accessing the native Windows Terminal Services API (now the Remote Desktop Services API). It can be used from C#, Visual Basic.NET, or any other .NET language.

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
First download and unzip the distribution and add a reference to Cassia (in the Bin folder), or add a reference to the [Cassia NuGet package](http://nuget.org/List/Packages/Cassia). Then you can immediately start writing cheesy Hello world applications like this one:
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

A sample console application demonstrating all of Cassia's functionality is included in the distribution (in the Samples\SessionInfo folder).

## What platforms are supported? ##
Cassia is supported on Windows XP, Vista, 7, and Windows Server 2003, 2003R2, 2008, and 2008R2. Most of the functions will work on Windows 2000 as well, but that platform is no longer officially supported.

## But what about the standard .NET Framework library? (Or: we don't need no stinkin' external libraries!) ##
Well, support for the Terminal Services API in the .NET Framework is rather limited. Specifically, the only Terminal Services operations supported in the .NET library are:
  1. [Process.SessionId](http://msdn.microsoft.com/en-us/library/system.diagnostics.process.sessionid.aspx), which retrieves the Terminal Services session ID for a given process.
  1. [ServiceBase.OnSessionChange](http://msdn.microsoft.com/en-us/library/system.serviceprocess.servicebase.onsessionchange.aspx), which provides notifications of changes to Terminal Services  sessions in a Windows service.
  1. [SystemEvents.SessionSwitch](http://msdn.microsoft.com/en-us/library/microsoft.win32.systemevents.sessionswitch.aspx), which provides notifications of changes to the session in which the application is running, provided that it is running a message loop.
  1. [SystemInformation.TerminalServerSession](http://msdn.microsoft.com/en-us/library/system.windows.forms.systeminformation.terminalserversession.aspx), which tells you whether the calling process is running in a Terminal Services session.

## Umm... ##
First of all, you'll want to consult the documentation, located in the Doc folder of the distribution. If that doesn't do it for you, post to the mailing list: [cassia-users@googlegroups.com](mailto:cassia-users@googlegroups.com). Bugs and feature requests can be submitted to the [issue tracker](http://code.google.com/p/cassia/issues/list).