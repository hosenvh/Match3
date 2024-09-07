using System;
using Match3.Foundation.Base.TaskManagement;
using Match3.Foundation.Unity.CohortManagement;

namespace Match3.Main
{
    public class ServerConfigUpdatingTask : BasicTask
    {
        protected override void InternalExecute(Action onComplete, Action onAbort)
        {
            if (GolmoradCohortManagementServerCommunicator.serverCofingIsUpdated)
                onComplete();
            else
                new ServerConfigRequest().CheckForUpdate((s) => onComplete());
        }
    }
}