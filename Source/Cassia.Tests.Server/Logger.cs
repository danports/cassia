using System;

namespace Cassia.Tests.Server
{
    public static class Logger
    {
        public static void InSessionLog(string text)
        {
            // TODO: this is a lame logging mechanism.
            Console.WriteLine(DateTime.Now + ": " + text);
        }

        public static void MainServerLog(string text)
        {
            Console.WriteLine(DateTime.Now + ": " + text);
        }
    }
}