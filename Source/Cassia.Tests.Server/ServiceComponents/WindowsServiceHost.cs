using System.ServiceProcess;

namespace Cassia.Tests.Server.ServiceComponents
{
    public class WindowsServiceHost : ServiceBase, IServiceHost
    {
        private IHostedService _service;

        #region IServiceHost Members

        public void Run(IHostedService service)
        {
            _service = service;
            _service.Attach(this);
            ServiceName = _service.Name;
            Run(this);
        }

        public void Log(string message)
        {
            EventLog.WriteEntry(message);
        }

        #endregion

        protected override void OnStart(string[] args)
        {
            _service.Start();
        }

        protected override void OnStop()
        {
            _service.Stop();
        }
    }
}