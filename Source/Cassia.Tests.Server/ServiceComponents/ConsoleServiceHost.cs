using System;

namespace Cassia.Tests.Server.ServiceComponents
{
    public class ConsoleServiceHost : IServiceHost
    {
        #region IServiceHost Members

        public void Run(IHostedService service)
        {
            service.Attach(this);
            Logger.Log(string.Format("Starting service {0}...", service.Name));
            service.Start();
            Logger.Log("Service started.");
            ConsoleKeyInfo key;
            do
            {
                key = Console.ReadKey(true);
            } while (key.Key != ConsoleKey.D || key.Modifiers != ConsoleModifiers.Control);
            Logger.Log(string.Format("Stopping service {0}...", service.Name));
            service.Stop();
            Logger.Log("Service stopped.");
        }

        #endregion
    }
}