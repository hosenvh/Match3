using Match3.Development.Base;
using Match3.Development.Base.DevelopmentConsole;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Game.PiggyBank;
using Match3.LuckySpinner.TimeBased.Game;
using UnityEngine;


namespace Match3.Development.DevOptions
{
    [DevOptionGroup(groupName: "Utility", priority: 10)]
    public class UtilityDevOptions : DevelopmentOptionsDefinition
    {
        [DevOption(commandName: "Force First Time Dynamic Offer")]
        public static void ForceFirstTimeDynamicSpecialOffer()
        {
            ForceDynamicSpecialOffer(-1);
        }

        [DevOption(commandName: "Force Show Dynamic Offer")]
        public static void ForceDynamicSpecialOffer(int offerIndex)
        {
            Object.FindObjectOfType<MainMenuDynamicSpecialOfferController>()?.ForceShowDynamicSpecialOffer(offerIndex);
        }

        [DevOption(commandName: "Add to Piggy Bank Coins *4")]
        public static void AddPiggyBankCoins(int coins)
        {
            ServiceLocator.Find<PiggyBankManager>().AddCredit(coins, delegate { });
        }

        [DevOption(commandName: "Set Piggy Bank Level (1-6)")]
        public static void SetPiggyBankLevel(int level)
        {
            level = Mathf.Clamp(level, min: 1, max: 6);
            ServiceLocator.Find<PiggyBankManager>().UpgradeBankLevel_TestMode(level);
        }

        [DevOption(commandName: "Spinner ResetTime", color: DevOptionColor.Yellow)]
        public static void ResetTimerBasedSpinner()
        {
            TimeBasedLuckySpinnerHandler.SetRemainingTime(0);
        }

        [DevOption(commandName: "Spinner Set RemainedTime")]
        public static void SetTimerBasedSpinner(int seconds)
        {
            TimeBasedLuckySpinnerHandler.SetRemainingTime(seconds);
        }
    }
}