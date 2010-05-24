using System;
using System.Threading;
using NUnit.Framework;

namespace Cassia.Tests
{
    [TestFixture]
    public class DisconnectTests
    {
        [Test]
        public void DisconnectOperationDisconnectsClient([TestServers] TestServer server)
        {
            using (ServerContext context = new ServerContext(server))
            {
                using (RdpConnection connection = context.OpenRdpConnection())
                {
                    ManualResetEvent disconnectEvent = new ManualResetEvent(false);
                    connection.Disconnected += delegate { disconnectEvent.Set(); };
                    context.TestService.Disconnect(connection.SessionId);
                    if (!disconnectEvent.WaitOne(TimeSpan.FromSeconds(10)))
                    {
                        throw new TimeoutException("Not disconnected yet");
                    }
                }
            }
        }

        [Test]
        public void DisconnectOperationUpdatesSessionStatus([TestServers] TestServer server)
        {
            using (ServerContext context = new ServerContext(server))
            {
                using (RdpConnection connection = context.OpenRdpConnection())
                {
                    context.TestService.Disconnect(connection.SessionId);
                    Assert.That(context.TestService.GetSessionState(connection.SessionId),
                                Is.EqualTo(ConnectionState.Disconnected));
                }
            }
        }
    }
}