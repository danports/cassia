using System;
using System.Threading;
using NUnit.Framework;

namespace Cassia.Tests
{
    [TestFixture]
    public class LogoffTests
    {
        [Test]
        public void OperationClosesSession([TestServers] ServerInfo server)
        {
            using (ServerConnection context = new ServerConnection(server))
            {
                using (RdpConnection connection = context.OpenRdpConnection())
                {
                    context.TestService.Logoff(connection.SessionId);
                    // Give Windows a bit of time to clean up the session.
                    Thread.Sleep(1000);
                    Assert.That(context.TestService.SessionExists(connection.SessionId), Is.False);
                }
            }
        }

        [Test]
        public void OperationDisconnectsClient([TestServers] ServerInfo server)
        {
            using (ServerConnection context = new ServerConnection(server))
            {
                using (RdpConnection connection = context.OpenRdpConnection())
                {
                    ManualResetEvent disconnectEvent = new ManualResetEvent(false);
                    connection.Disconnected += delegate { disconnectEvent.Set(); };
                    context.TestService.Logoff(connection.SessionId);
                    if (!disconnectEvent.WaitOne(TimeSpan.FromSeconds(10)))
                    {
                        throw new TimeoutException("Not disconnected yet");
                    }
                }
            }
        }
    }
}