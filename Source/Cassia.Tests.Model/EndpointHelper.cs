namespace Cassia.Tests.Model
{
    public static class EndpointHelper
    {
        public const int DefaultPort = 17876;

        public static string GetEndpointUri(string server, int port)
        {
            return string.Format("net.tcp://{0}:{1}/CassiaTestService", server, port);
        }
    }
}