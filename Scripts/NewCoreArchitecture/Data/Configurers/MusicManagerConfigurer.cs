using Match3.Foundation.Base.Configuration;
using Match3.Foundation.Unity.Configuration;
using UnityEngine;


[CreateAssetMenu(menuName = "Match3/Configurations/MusicManagerConfiguration")]
public class MusicManagerConfigurer : ScriptableConfiguration, Configurer<MusicManager>
{
    
    public MusicClipData[] clipDatas = default;

    public void Configure(MusicManager entity)
    {
        entity.Configure(clipDatas);
    }

    public override void RegisterSelf(ConfigurationManager manager)
    {
        manager.Register(this);
    }
}
