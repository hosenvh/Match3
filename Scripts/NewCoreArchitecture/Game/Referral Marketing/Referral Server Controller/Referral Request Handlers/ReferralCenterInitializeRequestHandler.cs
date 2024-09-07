using System;
using System.Collections.Generic;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Network;
using Match3.UserManagement.Main;
using Match3.UserManagement.ProfileName;
using NiceJson;
using UnityEngine;


namespace Match3.Game.ReferralMarketing.RequestHandling
{

    public class ReferralCenterInitializeRequestHandler : ReferralCenterRequestHandler
    {
        private const string GET_REFERRAL_INITIALIZE_DATA_URL = "/referral-code/api/v2/player/get-all";

        private Action<InitializeData> onInitializeDataRequestSucceed;
        private Action<InitializeFailureReason> onInitializeDataRequestFailed;


        private UserProfileNameManager UserProfileNameManager => ServiceLocator.Find<UserManagementService>().UserProfileNameManager;

        public void RequestInitializeData(Action<InitializeData> onRequestSucceed,
            Action<InitializeFailureReason> onRequestFailed)
        {
            onInitializeDataRequestSucceed = onRequestSucceed;
            onInitializeDataRequestFailed = onRequestFailed;

            var userName = Base.gameManager != null ? UserProfileNameManager.CurrentUserProfileName : "";

            HTTPRequestBuilder initializeDataRequest = new HTTPRequestBuilder()
                .SetURL($"{BaseUrl}{GET_REFERRAL_INITIALIZE_DATA_URL}")
                .SetType(HTTPRequestType.GET)
                .AddParameters("globalUniqueId", ServiceLocator.Find<IUserProfile>().GlobalUserId)
                .AddParameters("packageName", Application.identifier)
                .AddParameters("username", userName)
                .AddParameters("market", ServiceLocator.Find<IMarketManager>().GetMarketName())
                .AddParameters("env", "prod")
                .AddHeader("Content-Type", "application/json");


            RequestFromServer(initializeDataRequest.Build(), OnInitializeDataSuccessCallback,
                OnInitializeDataFailureCallback);
        }

        private void OnInitializeDataSuccessCallback(string response)
        {
            //Debug.Log($"###################### Initialize Request Handler -- Success -- {response}");

            var jsonResponse = JsonNode.ParseJsonString(response);

            if (jsonResponse.ContainsKey("msg"))
            {
                var msg = jsonResponse["msg"].ToString();
                if (msg.ToLower() == "ok")
                {
                    var initData = ExtractData(jsonResponse["data"]);
                    onInitializeDataRequestSucceed(initData);
                }
                else
                    onInitializeDataRequestFailed(InitializeFailureReason.ServerIssue);
            }
            else
                onInitializeDataRequestFailed(InitializeFailureReason.ServerIssue);
        }

        private void OnInitializeDataFailureCallback(string response)
        {
            //Debug.Log($"###################### Initialize Request Handler -- Failure -- {response}");

            if (IsNoInternetError(response))
                onInitializeDataRequestFailed(InitializeFailureReason.NetworkConnectionError);
            else
                onInitializeDataRequestFailed(InitializeFailureReason.ServerIssue);
        }


        private InitializeData ExtractData(JsonNode data)
        {
            var initData = JsonUtility.FromJson<InitializeData>(data.ToJsonString());

            var goalPrizesJsonArray = data["goalPrizes"] as JsonArray;
            if (goalPrizesJsonArray == null || goalPrizesJsonArray.Count == 0)
            {
                initData.goalPrizes = null;
                return initData;
            }

            List<GoalPrizeData> goalPrizeDatas = new List<GoalPrizeData>();
            List<Reward> rewards = new List<Reward>();

            foreach (var goalPrize in goalPrizesJsonArray)
            {
                rewards.Clear();

                if (goalPrize["prizes"] == null) continue;
                if (JsonNode.ParseJsonString(goalPrize["prizes"].ToJsonString())["prizesConfigs"] is JsonArray
                    prizesConfigs)
                {
                    foreach (var prizeConfig in prizesConfigs)
                    {
                        Reward reward = null;
                        switch (prizeConfig["name"].ToString())
                        {
                            case "Coin":
                                reward = new CoinReward(prizeConfig["count"]);
                                break;
                            case "AllBooster":
                                reward = new AllBoostersReward(prizeConfig["count"]);
                                break;
                        }

                        if (reward != null) rewards.Add(reward);
                    }
                }

                goalPrizeDatas.Add(new GoalPrizeData()
                {
                    goalId = goalPrize["goalId"],
                    reward = new ReferralReward() {rewards = rewards.ToArray()}
                });
            }

            initData.goalPrizes = goalPrizeDatas.ToArray();
            return initData;
        }
    }
}