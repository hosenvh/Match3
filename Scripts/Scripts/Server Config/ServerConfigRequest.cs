using Match3.Foundation.Base;
using Match3.Foundation.Base.CohortManagement;
using Match3.Foundation.Base.Configuration;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Network;
using SeganX;
using System;
using Match3.Utility.GolmoradLogging;
using Medrick.Foundation.Base.PlatformFunctionality;
using UnityEngine;

namespace Match3
{
    public class ServerConfigRequest
    {
        private Action<string> callback = null;
        private Action<string> getTimeCallBack = null;

        private string infoURL;
        private string timerUrl;

        public ServerConfigRequest()
        {
            ServiceLocator.Find<ConfigurationManager>().Configure(this);
        }

        public void SetServerURL(string url)
        {
            infoURL = $"{url}/info";
            timerUrl = $"{url}/time";
        }

        public void TryUpdateConfig(Action onSuccess, Action onFail)
        {
            if (Utilities.IsConnectedToInternet() == false)
            {
                onFail.Invoke();
                return;
            }
            CheckForUpdate(
                result =>
                {
                    if (result == "Failed")
                        onFail.Invoke();
                    else
                        onSuccess.Invoke();
                });
        }

        public void CheckForUpdate(Action<string> callback)
        {
            CheckUpdateState(callback);
        }

        private void CheckUpdateState(Action<string> callback)
        {
            if (Utilities.IsConnectedToInternet())
            {
                var storeManager = ServiceLocator.Find<StoreFunctionalityManager>();
                
                this.callback = callback;
                var body = new NiceJson.JsonObject();

                body.Add("packageName", Application.identifier);
                body.Add("env", "prod");
                body.Add("clientVersion", ServiceLocator.Find<PlatformFunctionalityManager>().VersionCode());
                body.Add("platform", "ANDROID");
                body.Add("market", storeManager.StoreName());
                body.Add("globalUniqueId", ServiceLocator.Find<IUserProfile>().GlobalUserId);
                body.Add("deviceId", SystemInfo.deviceUniqueIdentifier);
                body.Add("cohort", ServiceLocator.Find<UserCohortAssignmentManager>().AssignedCohortID());

                HTTPRequestBuilder updateRequest = new HTTPRequestBuilder();
                updateRequest.SetURL(infoURL).SetType(HTTPRequestType.POST).SetBody(body.ToJsonString());
                updateRequest.AddHeader("Content-Type", "application/json").SetTimeout(4);
                ServiceLocator.Find<ServerConnection>().Request(updateRequest.Build(), OnSuccessCallback, OnFailureCallback);
            }
            else
            {
                callback("OK");
            }

        }

        void OnSuccessCallback(string response)
        {
            var jsonResponse = NiceJson.JsonNode.ParseJsonString(response);
            
            if (jsonResponse.ContainsKey("msg"))
            {
                var msg = jsonResponse["msg"].ToString();
                if (msg.ToLower() == "ok")
                {
                    var data = JsonUtility.FromJson<ServerConfigData>(jsonResponse["data"].ToJsonString());
                    var serverConfigManager = ServiceLocator.Find<ServerConfigManager>();
                    
                    serverConfigManager.Update(data);

                    data = serverConfigManager.data;
                    callback(data.status);

                    CheckCohortConsistency(data);

                    return;
                }
            }
            callback("OK");
        }

        private void CheckCohortConsistency(ServerConfigData data)
        {
            var cohortManager = ServiceLocator.Find<UserCohortAssignmentManager>();

            if (data.cohort.Equals(cohortManager.AssignedCohortID()) == false)
            {
                // NOTE: DEF and Offline_1 cohorts are the same, but only Offline_1 is defined in client.
                if (data.cohort.Equals("DEF") && cohortManager.AssignedCohortID().Equals("Offline_1"))
                    return;

                DebugPro.LogError<ServerLogTag>($"Mismatch between Local and Server Cohort. Local:{cohortManager.AssignedCohortID()} Server:{data.cohort}");
                cohortManager.OverrideCohort(data.cohort);
            }
        }

        void OnFailureCallback(string response)
        {

            callback("Failed");
        }

        public void UpdateServerTime(Action<string> onComplete)
        {
            if (Utilities.IsConnectedToInternet())
            {
                getTimeCallBack = onComplete;

                HTTPRequestBuilder timeRequest = new HTTPRequestBuilder();
                timeRequest.SetURL(timerUrl).SetType(HTTPRequestType.POST).SetBody("aa");
                timeRequest.AddHeader("Content-Type", "application/json");
                ServiceLocator.Find<ServerConnection>().Request(timeRequest.Build(), OnGetTimeSuccess, OnGetTimeFail);
            }
            else
            {
                // TODO: Wny returning "ok" on no internet?
                onComplete("OK");
            }
        }

        void OnGetTimeSuccess(string response)
        {
            var jsonResponse = NiceJson.JsonNode.ParseJsonString(response);
            if (jsonResponse.ContainsKey("msg"))
            {
                var msg = jsonResponse["msg"].ToString();
                if (msg.ToLower() == "ok")
                {
                    var data = JsonUtility.FromJson<ServerConfigData>(jsonResponse["data"].ToJsonString());

                    var serverManager = ServiceLocator.Find<ServerConfigManager>();

                    serverManager.UpdateServerTime(data.serverTimeUtc);
                    //var serverData = serverManager.data;
                    //serverData.serverTimeUtc = data.serverTimeUtc;
                    //ServiceLocator.Find<ServerConfigManager>().Overwrite(serverData);

                    ServiceLocator.Find<ITimeManager>().RefreshTimeSyncWithServer(data.serverTimeUtc);
                    getTimeCallBack(data.status);
                    return;
                }
            }
            getTimeCallBack("OK");
        }

        void OnGetTimeFail(string response)
        {
            getTimeCallBack("Failed");
        }

    }
}
