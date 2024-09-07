using System;
using System.Collections.Generic;
using Match3;
using Match3.Foundation.Base.Configuration;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Game;
using Match3.Game.ServerData;
using Match3.Network;
using SeganX;
using UnityEngine;



public class GiftCodeRequestHandler
{

    private const string GiftCodeRequestUrl = "/gift-code/player/use";

    private Action<GiftCodeData> successGiftCodeRequestResult;
    private Action<GiftCodeRequestFailedResult>  failedGiftCodeRequestResult;

    private string serverUrl;


    public GiftCodeRequestHandler()
    {
        ServiceLocator.Find<ConfigurationManager>().Configure(this);
    }
    
    public void Request(string code, Action<GiftCodeData> successResult, Action<GiftCodeRequestFailedResult> failedResult)
    {
        if (Utilities.IsConnectedToInternet())
        {
            successGiftCodeRequestResult = successResult;
            failedGiftCodeRequestResult  = failedResult;
            
            var serverConnectionService = ServiceLocator.Find<ServerConnection>();
            
            var body = new NiceJson.JsonObject();
            body.Add("gamePackageName", Application.identifier);
            body.Add("env", "prod");
            body.Add("globalUniqueId", ServiceLocator.Find<IUserProfile>().GlobalUserId);
            body.Add("code", code);
            
            
            HTTPRequestBuilder gcRequest = new HTTPRequestBuilder()
            .SetURL($"{serverUrl}{GiftCodeRequestUrl}")
            .SetType(HTTPRequestType.POST)
            .SetBody(body.ToJsonString())
            .AddHeader("Content-Type", "application/json");

            serverConnectionService.Request(gcRequest.Build(), OnSuccessServerCallback, OnFailureServerCallback);
        }
        else
        {
            failedResult(new GiftCodeRequestFailedResult { msg = "fail", status = 0 });
        }
    }


    void OnSuccessServerCallback(string response)
    {
        var jsonResponse = NiceJson.JsonNode.ParseJsonString(response);
         
        if (jsonResponse.ContainsKey("msg"))
        {
            var msg = jsonResponse["msg"].ToString();
            if (msg.ToLower() == "ok")
            {
                GiftCodeRequestSuccessResult sData = JsonUtility.FromJson<GiftCodeRequestSuccessResult>(jsonResponse["data"].ToJsonString());

                List<Reward> rewards = RewardParsingUtilities.ConvertToRewards(sData.rewardsConfig.rewards);
                
                GiftCodeData.GiftCodeSetScenarioData setScenarioData = null;
                bool hasGoldenTicket = sData.rewardsConfig.HasGoldenTicket();
                if (sData.rewardsConfig.HasSetScenario())
                    setScenarioData = new GiftCodeData.GiftCodeSetScenarioData(sData.rewardsConfig.setScenario.targetScenarioIndex);
                    
                
                var giftCodeData = new GiftCodeData(sData.giftCodeName, rewards.ToArray(), setScenarioData, hasGoldenTicket);
                
                successGiftCodeRequestResult(giftCodeData);
            }
        }
        else
        {
            Debug.LogError(jsonResponse);
        }
    }

    void OnFailureServerCallback(string response)
    {
        var jsonResponse = NiceJson.JsonNode.ParseJsonString(response);
        
        if (jsonResponse.ContainsKey("msg"))
        {
            var msg = jsonResponse["msg"].ToString();
            if (msg.ToLower() != "ok")
            {
                var tempStatus =  NiceJson.JsonNode.ParseJsonString(jsonResponse["data"])["status"];
                var failedResult = new GiftCodeRequestFailedResult() {msg = msg, status = tempStatus};
                failedGiftCodeRequestResult(failedResult);
            }
        }
        else if (ServiceLocator.Find<ServerConnection>().IsTimeOut(response))
        {
            var failedResult = new GiftCodeRequestFailedResult
            {
                msg = "Time Out", 
                status = 1
            };
            failedGiftCodeRequestResult(failedResult);
        }
    }


    public void SetServerURL(string serverUrl)
    {
        this.serverUrl = serverUrl;
    }
    
    
}
