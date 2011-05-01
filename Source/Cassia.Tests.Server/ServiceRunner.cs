using System;
using Cassia.Tests.Server.ServiceComponents;

namespace Cassia.Tests.Server
{
    public static class ServiceRunner
    {
        public static void Run(IHostedService service, string[] args)
        {
            var consoleArg = Array.Find(args,
                                        arg => string.Equals(arg, "/console", StringComparison.CurrentCultureIgnoreCase));
            var host = consoleArg != null ? (IServiceHost) new ConsoleServiceHost() : new WindowsServiceHost();
            host.Run(service);
        }
    }
}