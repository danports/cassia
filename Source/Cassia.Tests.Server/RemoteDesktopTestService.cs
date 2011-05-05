using System;
using System.ServiceModel;
using System.Threading;
using Cassia.Tests.Model;
using Cassia.Tests.Server.InSession;

namespace Cassia.Tests.Server
{
    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    public class RemoteDesktopTestService : IRemoteDesktopTestService
    {
        private readonly ITerminalServicesManager _manager = new TerminalServicesManager();
        private RemoteMessageBoxResult _lastResult;

        #region IRemoteDesktopTestService Members

        public void Disconnect(ConnectionDetails connection, int sessionId)
        {
            using (ImpersonationHelper.Impersonate(connection))
            {
                using (var server = GetServer(connection.Server))
                {
                    server.Open();
                    server.GetSession(sessionId).Disconnect();
                }
            }
        }

        public int GetLatestSessionId()
        {
            ITerminalServicesSession latest = null;
            foreach (ITerminalServicesSession session in _manager.GetLocalServer().GetSessions())
            {
                if (latest == null || latest.ConnectTime == null ||
                    (session.ConnectTime != null && session.ConnectTime > latest.ConnectTime))
                {
                    latest = session;
                }
            }
            if (latest == null)
            {
                throw new InvalidOperationException("No connected sessions found");
            }
            return latest.SessionId;
        }

        public ConnectionState GetSessionState(ConnectionDetails connection, int sessionId)
        {
            using (ImpersonationHelper.Impersonate(connection))
            {
                using (var server = GetServer(connection.Server))
                {
                    server.Open();
                    return server.GetSession(sessionId).ConnectionState;
                }
            }
        }

        public void Logoff(ConnectionDetails connection, int sessionId)
        {
            using (ImpersonationHelper.Impersonate(connection))
            {
                using (var server = GetServer(connection.Server))
                {
                    server.Open();
                    server.GetSession(sessionId).Logoff();
                }
            }
        }

        public bool SessionExists(ConnectionDetails connection, int sessionId)
        {
            using (ImpersonationHelper.Impersonate(connection))
            {
                using (var server = GetServer(connection.Server))
                {
                    server.Open();
                    try
                    {
                        var session = server.GetSession(sessionId);
                        // Windows XP sometimes connects you to session 0, and that session still exists after
                        // logging the user off, as a disconnected console session but with no associated username.
                        return (session.ConnectionState != ConnectionState.Disconnected ||
                                !string.IsNullOrEmpty(session.UserName));
                    }
                    catch (Exception)
                    {
                        return false;
                    }
                }
            }
        }

        public RemoteMessageBoxResult GetLatestMessageBoxResponse()
        {
            return _lastResult;
        }

        public void ClickButtonInWindow(int sessionId, string windowTitle, string button)
        {
            using (var context = new InSessionServiceContext(sessionId))
            {
                context.Service.ClickButtonInWindow(windowTitle, button);
            }
        }

        public bool WindowWithTitleExists(int sessionId, string windowTitle)
        {
            using (var context = new InSessionServiceContext(sessionId))
            {
                return context.Service.WindowWithTitleExists(windowTitle);
            }
        }

        public void StartShowingMessageBox(ConnectionDetails connection, int sessionId, string windowTitle, string text,
                                           RemoteMessageBoxButtons buttons, TimeSpan timeout)
        {
            new MessageBoxShower(this, sessionId, connection, windowTitle, text, buttons, timeout).Show();
        }

        #endregion

        private void SetLastMessageBoxResult(RemoteMessageBoxResult result)
        {
            _lastResult = result;
        }

        private ITerminalServer GetServer(string server)
        {
            return string.IsNullOrEmpty(server) ? _manager.GetLocalServer() : _manager.GetRemoteServer(server);
        }

        #region Nested type: MessageBoxShower

        private class MessageBoxShower
        {
            private readonly RemoteMessageBoxButtons _buttons;
            private readonly ConnectionDetails _connection;
            private readonly RemoteDesktopTestService _service;
            private readonly int _sessionId;
            private readonly string _text;
            private readonly TimeSpan _timeout;
            private readonly string _title;

            public MessageBoxShower(RemoteDesktopTestService service, int sessionId, ConnectionDetails connection,
                                    string title, string text, RemoteMessageBoxButtons buttons, TimeSpan timeout)
            {
                _service = service;
                _sessionId = sessionId;
                _connection = connection;
                _title = title;
                _text = text;
                _buttons = buttons;
                _timeout = timeout;
            }

            public void Show()
            {
                new Thread(ShowCore).Start();
            }

            private void ShowCore()
            {
                using (ImpersonationHelper.Impersonate(_connection))
                {
                    using (var server = _service.GetServer(_connection.Server))
                    {
                        server.Open();
                        var session = server.GetSession(_sessionId);
                        var result = session.MessageBox(_text, _title, _buttons, RemoteMessageBoxIcon.Warning,
                                                        default(RemoteMessageBoxDefaultButton),
                                                        default(RemoteMessageBoxOptions), _timeout, true);
                        _service.SetLastMessageBoxResult(result);
                    }
                }
            }
        }

        #endregion
    }
}