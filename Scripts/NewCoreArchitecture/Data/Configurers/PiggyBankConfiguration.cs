using System.Collections.Generic;
using UnityEngine;
using Match3.Foundation.Unity.Configuration;
using Match3.Foundation.Base.Configuration;
using Match3.Game.PiggyBank;
using Match3.Data.PiggyBank;

namespace Match3.Data.Configuration
{
    [CreateAssetMenu(menuName = "Match3/PiggyBank/PiggyBankConfiguration")]
    public class PiggyBankConfiguration : ScriptableConfiguration, Configurer<PiggyBankManager>, Configurer<PiggyBankUnlocker>
    {
        [SerializeField] int unlockLevelIndex = 1;
        [SerializeField] int coinMultiplier = 4;
        [SerializeField] List<PiggyBankLevelInfo> levelInfoList = new List<PiggyBankLevelInfo>();

        public void Configure(PiggyBankManager entity)
        {
            entity.SetLevelInfoList(levelInfoList);
            entity.SetRewardMultiplier(coinMultiplier);
        }

        public void Configure(PiggyBankUnlocker entity)
        {
            entity.SetUnlockLevel(unlockLevelIndex);
        }

        public override void RegisterSelf(ConfigurationManager manager)
        {
            manager.Register<PiggyBankManager>(this);
            manager.Register<PiggyBankUnlocker>(this);
        }
    }
}