using System;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Network;
using NiceJson;
using UnityEngine;


namespace Match3.Game.ReferralMarketing.RequestHandling
{

    public class ReferralCenterClaimRewardRequestHandler : ReferralCenterRequestHandler
    {
        private const string CLAIM_PRIZE_URL = "/referral-code/player/claim";
        
        private Action onClaimPrizeRequestSucceed;
        private Action<ClaimRewardFailureReason> onClaimPrizeRequestFailed;

        private int claimingGoalId;
        
        public void RequestClaimPrize(int goalId, Action onClaimSucceed, Action<ClaimRewardFailureReason> onClaimFailed)
        {
            claimingGoalId = goalId;
            onClaimPrizeRequestSucceed = onClaimSucceed;
            onClaimPrizeRequestFailed = onClaimFailed;

            var body = new JsonObject
            {
                {"packageName", Application.identifier},
                {"env", "prod"},
                {"market", ServiceLocator.Find<IMarketManager>().GetMarketName()},
                {"globalUniqueId", ServiceLocator.Find<IUserProfile>().GlobalUserId},
                {"goalId", goalId}
            };

            HTTPRequestBuilder claimPrizeRequest = new HTTPRequestBuilder()
                .SetURL($"{BaseUrl}{CLAIM_PRIZE_URL}")
                .SetType(HTTPRequestType.POST)
                .SetBody(body.ToJsonString())
                .AddHeader("Content-Type", "application/json");
            
            RequestFromServer(claimPrizeRequest.Build(), OnClaimPrizeSuccessCallback,
                OnClaimPrizeFailureCallback);
        }

        private void OnClaimPrizeSuccessCallback(string response)
        {
            //Debug.Log($"###################### ClaimReward Request Handler -- Success -- {response}");
            
            var jsonResponse = JsonNode.ParseJsonString(response);

            if (jsonResponse.ContainsKey("msg"))
            {
                if (jsonResponse["msg"].ToString().ToLower()=="ok" && jsonResponse["data"]["goalId"]==claimingGoalId)
                    onClaimPrizeRequestSucceed();
                else
                    onClaimPrizeRequestFailed(ClaimRewardFailureReason.ServerIssue);
            }
            else
                onClaimPrizeRequestFailed(ClaimRewardFailureReason.ServerIssue);
        }

        private void OnClaimPrizeFailureCallback(string response)
        {
            //Debug.Log($"###################### ClaimReward Request Handler -- Failure -- {response}");
            
            if (IsNoInternetError(response) || IsTimeOutError(response))
            {
                onClaimPrizeRequestFailed(ClaimRewardFailureReason.NetworkConnectionError);
                return;
            }
            
            var jsonResponse = JsonNode.ParseJsonString(response);
            if (jsonResponse.ContainsKey("msg"))
            {
                var msg = jsonResponse["msg"].ToString().ToLower();

                if (msg.Equals("you_claim_before"))
                    onClaimPrizeRequestFailed(ClaimRewardFailureReason.AlreadyClaimed);
                else if (msg.Equals("not_enough_referred"))
                    onClaimPrizeRequestFailed(ClaimRewardFailureReason.NotEnoughReferred);
                else if (msg.Equals("referral_prize_goal_id_not_found"))
                    onClaimPrizeRequestFailed(ClaimRewardFailureReason.NotExists);
                else
                    onClaimPrizeRequestFailed(ClaimRewardFailureReason.ServerIssue);
            }
            else
                onClaimPrizeRequestFailed(ClaimRewardFailureReason.ServerIssue);
        }
        
        
    }
    
}

