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
        public void OperationUpdatesSessionStatus([TestConfigurations] ServerConfiguration config)
        {
            using (ServerContext context = new ServerContext(config))
            {
                using (var connection = context.OpenRdpConnection())
                {
                    context.Source.Disconnect(context.TargetConnection, connection.SessionId);
                    Assert.That(context.Source.GetSessionState(context.TargetConnection, connection.SessionId),
                                Is.EqualTo(ConnectionState.Disconnected));
                }
            }
        }
    }
}