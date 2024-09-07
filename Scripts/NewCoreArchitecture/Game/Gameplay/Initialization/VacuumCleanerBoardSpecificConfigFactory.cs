using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.Factories;
using System.Collections.Generic;

namespace Match3.Game.Gameplay.Initialization
{
    public class VacuumCleanerBoardSpecificConfigFactory : BoardSpecificConfigFactory<VacuumCleanerConfig, VacuumCleanerBoardSpecificConfig>
    {
        public VacuumCleanerBoardSpecificConfigFactory(BoardConfig boardConfig, TileFactory tileFactory, List<VacuumCleanerBoardSpecificConfig> dynamicConfigs) : base(boardConfig, tileFactory, dynamicConfigs)
        {
        }


        protected override Tile CreateFrom(VacuumCleanerConfig staticConfig, VacuumCleanerBoardSpecificConfig dynamicConfig)
        {
            return tileFactory.CreateVacuumCleaner(
             staticConfig.targetColor,
             dynamicConfig.targetAmount,
             dynamicConfig.direction,
             dynamicConfig.priority);
        }
    }
}