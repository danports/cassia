using System;
using System.Threading;
using System.Windows.Forms;
using AxMSTSCLib;

namespace Cassia.Tests
{
    public class RdpConnection : IDisposable
    {
        private readonly ManualResetEvent _connectedEvent;
        private readonly ServerContext _context;
        private readonly int _sessionId;
        private readonly Thread _thread;
        private AxMsRdpClient7NotSafeForScripting _ax;
        private TestForm _form;

        public RdpConnection(ServerContext context)
        {
            _context = context;

            _connectedEvent = new ManualResetEvent(false);
            _thread = new Thread((ThreadStart) delegate { ConnectCore(); });
            // The RDP ActiveX control requires STA.
            _thread.SetApartmentState(ApartmentState.STA);
            _thread.Start();
            if (!_connectedEvent.WaitOne(TimeSpan.FromSeconds(30)))
            {
                throw new TimeoutException("Could not connect to server " + _context.Server);
            }

            // Unfortunately, there doesn't seem to be any way to pull the session ID from the client,
            // so we have to pull it from the server.
            _sessionId = context.TestService.GetLatestSessionId();
        }

        public int SessionId
        {
            get { return _sessionId; }
        }

        public int Connected
        {
            get { return _ax.Connected; }
        }

        #region IDisposable Members

        public void Dispose()
        {
            _form.Invoke((ThreadStart) delegate { _form.Close(); });
            // Wait for the form to clean itself up.
            _thread.Join();
        }

        #endregion

        private void ConnectCore()
        {
            _form = new TestForm();
            _form.Show();
            _ax = _form.RdpControl;
            _ax.Server = _context.Server.Name;
            _ax.Domain = _context.Server.Domain;
            _ax.UserName = _context.Server.Username;
            _ax.AdvancedSettings8.ClearTextPassword = _context.Server.Password;
            _ax.OnConnected += delegate { _connectedEvent.Set(); };
            _ax.Connect();

            // You need a message loop or else the RDP client control will never connect.
            // That's why we run the form on a separate thread.
            Application.Run(_form);
            _form.Dispose();
        }
    }
}