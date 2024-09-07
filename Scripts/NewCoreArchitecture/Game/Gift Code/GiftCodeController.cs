using System;
using I2.Loc;
using Match3.Foundation.Base.EventManagement;
using Match3.Foundation.Base.ServiceLocating;


public struct GiftCodeRewardGivingStartedEvent : GameEvent {}
public struct GiftCodeRewardGivingFinishedEvent : GameEvent {}

public class GiftCodeController
{
    
    private GiftCodeError[] giftCodeErrors;
    
    public GiftCodeController()
    {
        giftCodeErrors = new[]
        {
            new GiftCodeError(0, ScriptLocalization.Message_Network.InternetConnectionFailedTryLater),
            new GiftCodeError(1, ScriptLocalization.Message_Network.InternetConnectionFailedTryLater),
            new GiftCodeError(40, ScriptLocalization.Message_GiftCode.UsedCode),
            new GiftCodeError(30, ScriptLocalization.Message_GiftCode.FullCapacity),
            new GiftCodeError(10, ScriptLocalization.Message_GiftCode.WrongCode),
            new GiftCodeError(20, ScriptLocalization.Message_GiftCode.ExpiredCode)
        };
    }
    

    public void RequestVerifyGiftCode(string code, Action<GiftCodeData> successResult, Action<string> failedResult)
    {
        var requestHandler = new GiftCodeRequestHandler();
        requestHandler.Request(code.ToUpper(), 
            
            result =>
            {
                ServiceLocator.Find<EventManager>().Propagate(new GiftCodeRewardGivingStartedEvent(), this);
                foreach (var reward in result.rewards)
                {
                    reward.Apply();
                }
                ServiceLocator.Find<EventManager>().Propagate(new GiftCodeRewardGivingFinishedEvent(), this);

                successResult(result);
            },

            result =>
            {
                var failString = GetFailVerifyingDescription(result.status);
                failedResult(string.IsNullOrEmpty(failString) ? result.msg : failString);
            } 
            
            );
    }


    private string GetFailVerifyingDescription(int statusCode)
    {
        foreach (var error in giftCodeErrors)
        {
            if (statusCode == error.statusCode)
            {
                return error.statusDescription;
            }
        }

        return "";
    }
    
    
    
}
