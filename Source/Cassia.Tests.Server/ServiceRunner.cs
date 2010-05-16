using System;

namespace Cassia.Tests.Server
{
    public static class ServiceRunner
    {
        public static void Run(IHostedService service, string[] args)
        {
            string consoleArg = Array.Find(args,
                                           delegate(string arg)
                                               {
                                                   return string.Equals(arg, "/console",
                                                                        StringComparison.CurrentCultureIgnoreCase);
                                               });
            IServiceHost host = consoleArg != null ? (IServiceHost) new ConsoleServiceHost() : new WindowsServiceHost();
            host.Run(service);
        }
    }
}