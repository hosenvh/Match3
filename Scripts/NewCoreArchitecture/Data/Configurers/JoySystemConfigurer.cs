using System.Collections;
using System.Collections.Generic;
using I2.Loc;
using Match3.Foundation.Base.Configuration;
using Match3.Foundation.Unity.Configuration;
using UnityEngine;

[CreateAssetMenu(menuName = "Match3/Configurations/JoySystemConfigurer")]
public class JoySystemConfigurer : ScriptableConfiguration, Configurer<JoySystem>
{
    public LocalizedStringTerm rateMessageLocalizedString;

    public void Configure(JoySystem entity)
    {
        entity.SetMarketRateMessage(rateMessageLocalizedString);
    }

    public override void RegisterSelf(ConfigurationManager manager)
    {
        manager.Register(this);
    }
    
    
}
