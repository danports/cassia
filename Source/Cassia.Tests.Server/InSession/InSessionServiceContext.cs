using System;
using System.Diagnostics;
using System.Reflection;
using System.ServiceModel;
using System.Threading;

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
            Logger.MainServerLog("Telling in session server to quit...");
            _testService.Stop();
            _process.WaitForExit();
            Logger.MainServerLog("In session server has exited");
        }

        private void StartService()
        {
            Logger.MainServerLog("Starting in session server for session " + _sessionId);

            // TODO: For the process to start, the user that is connected to the session needs to have
            // access to the C:\Windows\Temp folder WITHOUT running as an administrator.
            // Should probably just run the process as an admin to fix this.
            _process = ProcessHelper.Start(_sessionId, Assembly.GetEntryAssembly().Location, Program.InSessionSwitch);
            Thread.Sleep(2000); // Give it some time to start up (TODO: is this necessary?)

            // Using the named pipes binding here is problematic, as described here:
            // http://weblogs.thinktecture.com/cweyer/2007/12/dealing-with-os-privilege-issues-in-wcf-named-pipes-scenarios.html
            // Essentially, the process that we just started is running as a limited user,
            // and limited users cannot write to the shared memory location where WCF stores the 
            // endpoint name to pipe GUID mappings.
            _testService = ChannelFactory<IInSessionTestService>.CreateChannel(new NetTcpBinding(),
                                                                               new EndpointAddress(
                                                                                   InSessionServer.EndpointUri));
        }
    }
}