using System.Threading;
using NUnit.Framework;

namespace Cassia.Tests
{
    [TestFixture]
    public class DisconnectTests
    {
        [Test]
        public void DisconnectOperationDisconnectsClient([ValueSource(typeof(TestEnvironment), "Servers")] string server)
        {
            using (ServerContext context = new ServerContext(server))
            {
                using (RdpConnection connection = context.OpenRdpConnection())
                {
                    context.TestService.Disconnect(connection.SessionId);
                    Thread.Sleep(1000);
                    Assert.That(connection.Connected, Is.EqualTo(0));
                }
            }
        }
    }
}