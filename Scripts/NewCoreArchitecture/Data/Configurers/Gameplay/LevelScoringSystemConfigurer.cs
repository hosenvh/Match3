using Match3.Foundation.Base.Configuration;
using Match3.Foundation.Unity.Configuration;
using UnityEngine;
using Match3.Game.Gameplay.SubSystems.LevelEnding;

namespace Match3.Data.Configuration
{
    [CreateAssetMenu(menuName = "Match3/Configurations/Gameplay/" + nameof(LevelScoringSystemConfigurer))]
    public class LevelScoringSystemConfigurer : ScriptableConfiguration, Configurer<LevelFinalScoringSystem>
    {
        public int normalBaseScore;
        public int hardBaseScore;
        public int veryHardBaseScore;

        public override void RegisterSelf(ConfigurationManager manager)
        {
            manager.Register(this);
        }

        public void Configure(LevelFinalScoringSystem entity)
        {
            entity.SetBaseDifficultyScore(DifficultyLevel.Normal, normalBaseScore);
            entity.SetBaseDifficultyScore(DifficultyLevel.Hard, hardBaseScore);
            entity.SetBaseDifficultyScore(DifficultyLevel.VeryHard, veryHardBaseScore);
        }
    }
}