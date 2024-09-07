using System;
using System.Collections.Generic;
using I2.Loc;
using Match3.Game;
using Match3.Profiler;

namespace Match3.LevelInfoAds.RewardHandler
{
    public class LevelInfoAdsPowerupRewardHandler : LevelInfoAdsRewardHandler
    {
        protected override List<Type> GetPossibleRewardsTypes()
        {
            return new List<Type>
            {
                typeof(HammerPowerUpReward),
                typeof(BroomPowerUpReward),
                typeof(HandPowerUpReward)
            };
        }

        protected override LevelReservedRewardsHandler GetCorrespondingLevelReservedRewardHandler()
        {
            return Base.gameManager.levelSessionProfiler.PowerUpsReservedRewardsHandler;
        }

        public override string GetRewardConfirmPopupMessage()
        {
            return string.Format(ScriptLocalization.UI_LevelInfo.YouGotARewardForThisLevel, ScriptLocalization.UI_General.Powerup);
        }
    }
}