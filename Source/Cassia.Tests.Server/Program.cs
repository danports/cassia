namespace Cassia.Tests.Server
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            ServiceRunner.Run(new TestServer(), args);
        }
    }
}