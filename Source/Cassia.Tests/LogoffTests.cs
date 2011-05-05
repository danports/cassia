using System;
using System.Threading;
using NUnit.Framework;

namespace Cassia.Tests
{
    [TestFixture]
    public class LogoffTests
    {
        [Test]
        public void OperationClosesSession([TestConfigurations] ServerConfiguration config)
        {
            using (var context = new ServerContext(config))
            {
                using (var connection = context.OpenRdpConnection())
                {
                    context.Source.Logoff(context.TargetConnection, connection.SessionId);
                    // Give Windows a bit of time to clean up the session.
                    Thread.Sleep(1000);
                    Assert.That(context.Source.SessionExists(context.TargetConnection, connection.SessionId), Is.False);
                }
            }
        }

        [Test]
        public void OperationDisconnectsClient([TestConfigurations] ServerConfiguration config)
        {
            using (var context = new ServerContext(config))
            {
                using (var connection = context.OpenRdpConnection())
                {
                    var disconnectEvent = new ManualResetEvent(false);
                    connection.Disconnected += delegate { disconnectEvent.Set(); };
                    context.Source.Logoff(context.TargetConnection, connection.SessionId);
                    if (!disconnectEvent.WaitOne(TimeSpan.FromSeconds(10)))
                    {
                        throw new TimeoutException("Not disconnected yet");
                    }
                }
            }
        }
    }
}