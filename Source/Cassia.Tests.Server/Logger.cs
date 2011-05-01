namespace Cassia.Tests.Server
{
    public static class Logger
    {
        private static ILogger _current = new ConsoleLogger();

        public static void SetLogger(ILogger logger)
        {
            _current = logger;
        }

        public static void Log(string text)
        {
            _current.Log(text);
        }
    }
}