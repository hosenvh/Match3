

using Match3.Foundation.Base.ServiceLocating;
using NiceJson;
using System;

namespace Match3.Foundation.Base.CohortManagement
{

    public interface CohortManagementServerCommunicator : Service
    {
        void RequestUserCohort(Action<string> onSuccess, Action onFailure);
        void RequestBalanceConfig(string cohortID, Action<JsonObject> onSuccess, Action onFailure);
        void SendUserCohort(string cohortID, Action onSuccess, Action onFailure);
        void RequestResetCohort(string cohortID);
    }
}