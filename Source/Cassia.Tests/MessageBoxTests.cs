using System;
using NUnit.Framework;

namespace Cassia.Tests
{
    [TestFixture]
    public class MessageBoxTests
    {
        [Test]
        public void ShowMessageBoxAndGetResponse([TestConfigurations] ServerConfiguration config)
        {
            using (var context = new ServerContext(config))
            {
                using (var connection = context.OpenRdpConnection())
                {
                    var title = "Test " + Guid.NewGuid();
                    context.Source.StartShowingMessageBox(context.TargetConnection, connection.SessionId, title,
                                                          "Hello!", RemoteMessageBoxButtons.AbortRetryIgnore,
                                                          TimeSpan.Zero);
                    Assert.That(context.Target.WindowWithTitleExists(connection.SessionId, title));
                    const RemoteMessageBoxResult expected = RemoteMessageBoxResult.Retry;
                    // The automation ID of the button in the window is 4.
                    context.Target.ClickButtonInWindow(connection.SessionId, title, ((int) expected).ToString());
                    Assert.That(context.Source.GetLatestMessageBoxResponse(), Is.EqualTo(expected));
                }
            }
        }
    }
}