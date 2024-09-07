using Match3.Foundation.Base.Configuration;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Foundation.Unity.TimeManagement;
using NiceJson;
using System;
using Match3.Utility.GolmoradLogging;
using Match3.UserManagement.Foundation.Base;
using UnityEngine;

namespace Match3.Foundation.Base.CohortManagement
{
    public class FakeCohortManagementServerCommunicator : CohortManagementServerCommunicator
    {

        public enum RequestResult { Fail, Success}

        public RequestResult balanceRequestResult;
        public JsonObject balanceConfig;

        public RequestResult cohortRequestResult;
        public string cohortID;

        public float communicationDelay;

        public FakeCohortManagementServerCommunicator()
        {
            ServiceLocator.Find<ConfigurationManager>().Configure(this);
            DebugPro.LogError<UserLogTag>("Using FakeSplitTestServerCommunicator. Please don't use this in the production release");
        }

        public void RequestBalanceConfig(string cohortID, Action<JsonObject> onSuccess, Action onFailure)
        {
            Action action = () =>
            {
                if (balanceRequestResult == RequestResult.Fail)
                    onFailure();
                else
                    onSuccess(balanceConfig);
            };

            ProcessCommunication(action);
        }

        private void ProcessCommunication(Action action)
        {
            if (communicationDelay > 0)
                ServiceLocator.Find<UnityTimeScheduler>().Schedule(communicationDelay, action, this);
            else
                action();
        }

        public void RequestUserCohort(Action<string> onSuccess, Action onFailure)
        {
            Action action = () =>
            {
                if (cohortRequestResult == RequestResult.Fail)
                    onFailure();
                else
                    onSuccess(cohortID);
            };

            ProcessCommunication(action);
        }

        public void SendUserCohort(string cohortID, Action onSuccess, Action onFailure)
        {
            onSuccess.Invoke();
        }

        public void RequestResetCohort(string cohortID)
        {
            throw new NotImplementedException();
        }
    }
}