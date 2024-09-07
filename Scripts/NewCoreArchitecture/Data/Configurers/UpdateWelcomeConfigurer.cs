using System.Collections.Generic;
using Match3.Foundation.Base.Configuration;
using Match3.Foundation.Unity.Configuration;
using Match3.Game.UpdateWelcome;
using UnityEngine;


namespace Match3.Data.UpdateWelcome
{

    [CreateAssetMenu(menuName = "Match3/Configurations/UpdateWelcomeConfigurer")]
    public class UpdateWelcomeConfigurer : ScriptableConfiguration, Configurer<UpdateWelcomeController>
    {

        public int updateCoinRewardCount;
        public List<ChangeLog> changeLogs;

        public void Configure(UpdateWelcomeController entity)
        {
            entity.SetUpdateCoinReward(updateCoinRewardCount);
            entity.SetChangeLogs(changeLogs);
        }

        public override void RegisterSelf(ConfigurationManager manager)
        {
            manager.Register(this);
        }
    }
    
}


