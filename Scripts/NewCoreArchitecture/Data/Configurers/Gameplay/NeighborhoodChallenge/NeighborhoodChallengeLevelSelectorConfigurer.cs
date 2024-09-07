using Match3.Foundation.Base.Configuration;
using Match3.Foundation.Unity.Configuration;
using UnityEngine;

[CreateAssetMenu(menuName = "Match3/Configurations/Gameplay/NC/LevelSelector")]
public class NeighborhoodChallengeLevelSelectorConfigurer : ScriptableConfiguration, Configurer<NeighborhoodChallengeLevelSelector>
{

    public float normalDifficulty;
    public int selectionLevelOffset;
    public float defaultDifficulty;
    public float[] levelsDifficulty;
    
    public void Configure(NeighborhoodChallengeLevelSelector entity)
    {
        entity.SetLevelsDifficulty(levelsDifficulty);
        entity.SetDifficultyData(normalDifficulty, defaultDifficulty);
        entity.SetSelectionLevelOffset(selectionLevelOffset);
    }

    public override void RegisterSelf(ConfigurationManager manager)
    {
        manager.Register(this);
    }
}
