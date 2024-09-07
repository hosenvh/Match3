using Match3.Foundation.Base;
using Match3.Foundation.Base.CohortManagement;
using Match3.Foundation.Base.Configuration;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Network;
using Medrick.Foundation.Base.PlatformFunctionality;
using NiceJson;
using System;
using Match3.Utility.GolmoradLogging;
using Match3.UserManagement.Foundation.Base;
using UnityEngine;


namespace Match3.Foundation.Unity.CohortManagement
{
    public class GolmoradCohortManagementServerCommunicator : CohortManagementServerCommunicator
    {
        public static bool serverCofingIsUpdated = false;

        string serverURL;

        public GolmoradCohortManagementServerCommunicator()
        {
            ServiceLocator.Find<ConfigurationManager>().Configure(this);
        }

        public void SetServerURL(string serverURL)
        {
            this.serverURL = serverURL;
        }

        public void RequestUserCohort(Action<string> onSuccess, Action onFailure)
        {
            var jsonObject = CreateDefaultData();

            var requestBuilder = new HTTPRequestBuilder()
                .AddHeader("Content-Type", "application/json")
                .SetType(HTTPRequestType.POST)
                .SetBody(jsonObject.ToJsonString())
                .SetURL($"{serverURL}/info");

            AddDefaultHeadersTo(requestBuilder);

            ServiceLocator.Find<ServerConnection>().Request(
                requestBuilder.Build(), 
                (r) => TryExtractCohort(r, onSuccess, onFailure), 
                (e) => onFailure());
        }


        private void TryExtractCohort(string rawResponse, Action<string> onSuccess, Action onFailure)
        {
            string cohortID = "";
            try
            {
                var response = JsonNode.ParseJsonString(rawResponse);
                cohortID = response["data"]["cohort"];

                // NOTE: This is only optimizing the server requests. Since Cohort and ServerConfig requests are the same. Try to find a better solution.
                var data = JsonUtility.FromJson<ServerConfigData>(response["data"].ToJsonString());
                ServiceLocator.Find<ServerConfigManager>().Update(data);
                serverCofingIsUpdated = true;

                SetSentCohort(cohortID);
            }
            catch(Exception e)
            {
                DebugPro.LogException<UserLogTag>(e);
                onFailure();
                return;
            }

            onSuccess(cohortID);
        }

        public void RequestBalanceConfig(string cohortId, Action<JsonObject> onSuccess, Action onFailure)
        {
        }

        public void SendUserCohort(string cohortID, Action onSuccess, Action onFailure)
        {
            if (IsAlreadySent(cohortID))
            {
                onSuccess.Invoke();
                return;
            }

            var jsonObject = CreateDefaultData();
            jsonObject.Add("cohort", cohortID);

            var requestBuilder = new HTTPRequestBuilder()
                .SetType(HTTPRequestType.POST)
                .SetURL($"{serverURL}/info")
                .SetBody(jsonObject.ToJsonString())
                .AddHeader("Content-Type", "application/json");

            AddDefaultHeadersTo(requestBuilder);

            ServiceLocator.Find<ServerConnection>().Request(
                requestBuilder.Build(),
                (s) => { SetSentCohort(cohortID); onSuccess.Invoke(); },
                (e) => { onFailure.Invoke(); });
        }

        private bool IsAlreadySent(string cohortID)
        {
            return cohortID.Equals(PlayerPrefs.GetString("GolmoradCohortManagement_SentCohort", ""));
        }

        private void SetSentCohort(string cohortID)
        {
            PlayerPrefs.SetString("GolmoradCohortManagement_SentCohort", cohortID);
        }

        // TODO: Refactor this.
        private void TryExtractConfig(string rawResponse, Action<JsonObject> onSuccess, Action onFailure)
        {
            JsonObject configData = null;
            try
            {
                var response = JsonNode.ParseJsonString(rawResponse);
                if (response["msg"].ToString().Equals("ok") == false)
                {
                    onFailure();
                    return;
                }
                configData = response["data"]["config"].As<JsonObject>();
            }
            catch(Exception e)
            {
                DebugPro.LogException<UserLogTag>(e);
                onFailure();
                return;
            }

            onSuccess(configData);
        }

        private HTTPRequestBuilder AddDefaultHeadersTo(HTTPRequestBuilder builder)
        {
            return builder;
            //return builder
            //    .AddHeader("client-version", ServiceLocator.Find<PlatformFunctionalityManager>().VersionCode().ToString())
            //    .AddHeader("market", MarketCode());
        }


        public JsonObject CreateDefaultData()
        {
            var jsonObject = new JsonObject();

            jsonObject.Add("packageName", Application.identifier);
            jsonObject.Add("env", "prod");
            jsonObject.Add("clientVersion", ServiceLocator.Find<PlatformFunctionalityManager>().VersionCode());
            jsonObject.Add("platform", "ANDROID");
            jsonObject.Add("market", ServiceLocator.Find<StoreFunctionalityManager>().StoreName());
            jsonObject.Add("globalUniqueId", ServiceLocator.Find<IUserProfile>().GlobalUserId);
            jsonObject.Add("deviceId", SystemInfo.deviceUniqueIdentifier);

            return jsonObject;
        }

        public void RequestResetCohort(string cohortID)
        {
            var jsonObject = CreateDefaultData();
            jsonObject.Add("cohort", cohortID);

            var requestBuilder = new HTTPRequestBuilder()
                .SetType(HTTPRequestType.POST)
                .SetURL($"{serverURL}/player/cohort/force-update")
                .SetBody(jsonObject.ToJsonString())
                .AddHeader("Content-Type", "application/json");

            AddDefaultHeadersTo(requestBuilder);

            ServiceLocator.Find<ServerConnection>().Request(
                requestBuilder.Build(),
                (s) => { },
                (e) => { });
        }
    }
}