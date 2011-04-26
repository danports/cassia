using System;
using System.Threading;
using NUnit.Framework;

namespace Cassia.Tests
{
    [TestFixture]
    public class DisconnectTests
    {
        [Test]
        public void OperationDisconnectsClient([TestConfigurations] ServerConfiguration config)
        {
            using (var context = new ServerContext(config))
            {
                using (var connection = context.OpenRdpConnection())
                {
                    var disconnectEvent = new ManualResetEvent(false);
                    connection.Disconnected += delegate { disconnectEvent.Set(); };
                    context.Source.Disconnect(context.TargetConnection, connection.SessionId);
                    if (!disconnectEvent.WaitOne(TimeSpan.FromSeconds(10)))
                    {
                        throw new TimeoutException("Not disconnected yet");
                    }
                }
            }
        }

        [Test]
        public void OperationUpdatesSessionStatus([TestServers] ServerInfo server)
        {
            using (var context = new ServerConnection(server))
            {
                using (var connection = context.OpenRdpConnection())
                {
                    context.TestService.Disconnect(null, connection.SessionId);
                    Assert.That(context.TestService.GetSessionState(connection.SessionId),
                                Is.EqualTo(ConnectionState.Disconnected));
                }
            }
        }
    }
}