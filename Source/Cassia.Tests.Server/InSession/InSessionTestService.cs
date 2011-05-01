using System;
using System.Runtime.InteropServices;
using System.ServiceModel;
using System.Windows.Automation;

namespace Cassia.Tests.Server.InSession
{
    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    public class InSessionTestService : IInSessionTestService
    {
        #region IInSessionTestService Members

        public void ClickButtonInWindow(string windowTitle, string button)
        {
            Logger.InSessionLog("trying to click button " + button + " in window " + windowTitle);
            var hwnd = FindWindow(null, windowTitle);
            if (hwnd == IntPtr.Zero)
            {
                return;
            }
            Logger.InSessionLog("got window; trying to find button");

            var element = AutomationElement.FromHandle(hwnd);
            foreach (AutomationElement child in
                element.FindAll(TreeScope.Children,
                                new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Button)))
            {
                Logger.InSessionLog(child.Current.Name + ": " + child.Current.AutomationId);
                if (child.Current.AutomationId != button)
                {
                    continue;
                }
                Logger.InSessionLog("found button! clicking");
                var pattern = (InvokePattern) child.GetCurrentPattern(InvokePattern.Pattern);
                pattern.Invoke();
                break;
            }
        }

        public bool WindowWithTitleExists(string windowTitle)
        {
            Logger.InSessionLog("trying to find window with title: " + windowTitle);
            return FindWindow(null, windowTitle) != IntPtr.Zero;
        }

        public void Stop()
        {
            // We can't very well just kill the WCF host in the middle of a call; that would make the caller
            // rather unhappy. So we need to delay the shutdown a bit, until this call has completed.
            // TODO: Is there a nicer way to do this? (e.g. listen to an event for call complete and shut down
            // when that is fired)
            Logger.InSessionLog("told to stop the insession server");
            InSessionServer.Stop();
        }

        #endregion

        [DllImport("User32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr FindWindow(string className, string windowName);
    }
}