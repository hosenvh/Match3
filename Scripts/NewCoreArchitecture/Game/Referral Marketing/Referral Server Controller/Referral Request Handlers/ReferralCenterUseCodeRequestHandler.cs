using System;
using System.Collections;
using System.Collections.Generic;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Network;
using NiceJson;
using SeganX;
using UnityEngine;


namespace Match3.Game.ReferralMarketing.RequestHandling
{
    
    public class ReferralCenterUseCodeRequestHandler : ReferralCenterRequestHandler
    {
        private const string USE_REFERRAL_CODE_URL = "/referral-code/player/use";
        
        private Action<SuccessfulReferredData> onUseReferralCodeRequestSucceed;
        private Action<UseReferralCodeFailureReason> onUseReferralCodeRequestFailed;
        
        
        public void UseReferralCode(string code, Action<SuccessfulReferredData> onRequestSucceed, Action<UseReferralCodeFailureReason> onRequestFailed)
        {
            onUseReferralCodeRequestSucceed = onRequestSucceed;
            onUseReferralCodeRequestFailed = onRequestFailed;

            var body = new JsonObject
            {
                {"packageName", Application.identifier},
                {"env", "prod"},
                {"market", ServiceLocator.Find<IMarketManager>().GetMarketName()},
                {"globalUniqueId", ServiceLocator.Find<IUserProfile>().GlobalUserId},
                {"referralCode", code}
            };

            HTTPRequestBuilder useCodeRequest = new HTTPRequestBuilder()
                .SetURL($"{BaseUrl}{USE_REFERRAL_CODE_URL}")
                .SetType(HTTPRequestType.POST)
                .SetBody(body.ToJsonString())
                .AddHeader("Content-Type", "application/json");

            RequestFromServer(useCodeRequest.Build(), OnUseReferralCodeSuccessCallback,
                OnUseReferralCodeFailureCallback);
        }

        private void OnUseReferralCodeSuccessCallback(string response)
        {
            //Debug.Log($"###################### UseCode Request Handler -- Success -- {response}");
            
            var jsonResponse = JsonNode.ParseJsonString(response);
         
            if (jsonResponse.ContainsKey("msg"))
            {
                var msg = jsonResponse["msg"].ToString();
                if (msg.ToLower() == "ok")
                {
                    onUseReferralCodeRequestSucceed(ExtractReferredData(jsonResponse["data"]));
                }
                else
                    onUseReferralCodeRequestFailed(UseReferralCodeFailureReason.ServerIssue);
            }
            else
            {
                onUseReferralCodeRequestFailed(UseReferralCodeFailureReason.ServerIssue);
            }
        }
        
        private void OnUseReferralCodeFailureCallback(string response)
        {
            //Debug.Log($"###################### UseCode Request Handler -- Failure -- {response}");
            
            if (string.IsNullOrEmpty(response))
            {
                onUseReferralCodeRequestFailed(UseReferralCodeFailureReason.ServerIssue);
                Debug.LogError("Use Referral Code Failure Response Is Null!");
                return;
            }
            
            if (IsNoInternetError(response) || IsTimeOutError(response))
            {
                onUseReferralCodeRequestFailed(UseReferralCodeFailureReason.NetworkConnectionError);
                return;
            }
            
            var jsonResponse = JsonNode.ParseJsonString(response);
            if (jsonResponse.ContainsKey("msg"))
            {
                var msg = jsonResponse["msg"].ToString().ToLower();

                if (msg.Equals("already_referred"))
                    onUseReferralCodeRequestFailed(UseReferralCodeFailureReason.AlreadyReferred);
                else if (msg.Equals("not_found"))
                    onUseReferralCodeRequestFailed(UseReferralCodeFailureReason.IncorrectCode);
                else
                    onUseReferralCodeRequestFailed(UseReferralCodeFailureReason.ServerIssue);
            }
            else
                onUseReferralCodeRequestFailed(UseReferralCodeFailureReason.ServerIssue);
        }


        private SuccessfulReferredData ExtractReferredData(JsonNode dataNode)
        {
            SuccessfulReferredData referredData = new SuccessfulReferredData
            {
                inviterUserName = dataNode["usernameReferred"],
                reward = new ReferralReward()
            };

            var rewardsConfig = JsonNode.ParseJsonString(dataNode["referralPrize"].ToJsonString())["prizesConfigs"];
            List<Reward> rewards = new List<Reward>();
            foreach (var rewardConfig in (JsonArray) rewardsConfig)
            {
                Reward reward = null;
                switch (rewardConfig["name"].ToString())
                {
                    case "Coin":
                        reward = new CoinReward(rewardConfig["count"]);
                        break;
                    case "AllBooster":
                        reward = new AllBoostersReward(rewardConfig["count"]);
                        break;
                }
                if(reward!=null) rewards.Add(reward);
            }

            referredData.reward.rewards = rewards.ToArray();
            return referredData;
        }
        
        
        
    }

}


