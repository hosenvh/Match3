using Match3.Foundation.Base.Configuration;
using Match3.Foundation.Unity.Configuration;
using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.SubSystems.LevelContinuing;
using PandasCanPlay.HexaWord.Utility;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

namespace Match3.Data.Configuration
{
    [CreateAssetMenu(menuName = "Match3/Configurations/Gameplay/" + nameof(LevelContinuingSystemConfigurer))]
    public class LevelContinuingSystemConfigurer : ScriptableConfiguration, Configurer<LevelContinuingSystem>
    {
        [Serializable]
        public struct LevelContinuingStageData
        {
            public int neededCoins;
            public int additionalMoves;

            [Type(typeof(Tile), includeAbstracts :false)]
            public List<string> tilesToCreate;

            public LevelContinuingStage ConvertToLogicalEntity()
            {
                return new LevelContinuingStage(
                    neededCoins, 
                    additionalMoves, 
                    tilesToCreate.Select(name => Type.GetType(name)).ToList());
            }
        }

        public List<LevelContinuingStageData> stages;


        public void Configure(LevelContinuingSystem entity)
        {
            foreach (var stage in stages)
                entity.AddStage(stage.ConvertToLogicalEntity());
        }

        public override void RegisterSelf(ConfigurationManager manager)
        {
            manager.Register(this);
        }
    }
}