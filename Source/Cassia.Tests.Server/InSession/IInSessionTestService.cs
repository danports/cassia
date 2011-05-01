using System.ServiceModel;

namespace Cassia.Tests.Server.InSession
{
    [ServiceContract]
    public interface IInSessionTestService
    {
        [OperationContract]
        void ClickButtonInWindow(string windowTitle, string button);

        [OperationContract]
        bool WindowWithTitleExists(string windowTitle);

        [OperationContract(IsOneWay = true)]
        void Stop();
    }
}