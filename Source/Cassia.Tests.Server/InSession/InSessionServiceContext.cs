using System;
using System.Diagnostics;
using System.Reflection;
using System.ServiceModel;

namespace Cassia.Tests.Server.InSession
{
    public class InSessionServiceContext : IDisposable
    {
        private readonly int _sessionId;
        private Process _process;
        private IInSessionTestService _testService;

        public InSessionServiceContext(int sessionId)
        {
            _sessionId = sessionId;
        }

        public IInSessionTestService Service
        {
            get
            {
                if (_testService == null)
                {
                    StartService();
                }
                return _testService;
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            StopService();
        }

        #endregion

        private void StopService()
        {
            if (_testService == null)
            {
                return;
            }
            Logger.Log("Telling in session server to quit...");
            _testService.Stop();
            _process.WaitForExit();
            Logger.Log("In session server has exited");
        }

        private void StartService()
        {
            Logger.Log("Starting in session server for session " + _sessionId);
            _process = ProcessHelper.Start(_sessionId, Assembly.GetEntryAssembly().Location, Program.InSessionSwitch);

            // TODO: now that we are running as an administrator, we could probably use the named pipes binding.
            // Using the named pipes binding here is problematic when running as a limited user, as described here:
            // http://weblogs.thinktecture.com/cweyer/2007/12/dealing-with-os-privilege-issues-in-wcf-named-pipes-scenarios.html
            _testService = ChannelFactory<IInSessionTestService>.CreateChannel(new NetTcpBinding(),
                                                                               new EndpointAddress(
                                                                                   InSessionServer.EndpointUri));
        }
    }
}