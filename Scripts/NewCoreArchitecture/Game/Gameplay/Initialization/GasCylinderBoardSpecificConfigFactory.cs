using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.Factories;
using System.Collections.Generic;

namespace Match3.Game.Gameplay.Initialization
{
    public class GasCylinderBoardSpecificConfigFactory : BoardSpecificConfigFactory<GasCylinderConfig, GasCylinderBoardSpecificConfig>
    {
        public GasCylinderBoardSpecificConfigFactory(BoardConfig boardConfig, TileFactory tileFactory, List<GasCylinderBoardSpecificConfig> dynamicConfigs) : base(boardConfig, tileFactory, dynamicConfigs)
        {
        }


        protected override Tile CreateFrom(GasCylinderConfig staticConfig, GasCylinderBoardSpecificConfig dynamicConfig)
        {
            return tileFactory.CreateGasCylinder(staticConfig.color, boardConfig.TEMPORARY_InBoardGasCylinderStartCountdown);
            //return tileFactory.CreateGasCylinder(staticConfig.color, dynamicConfig.startCountdown);
        }
    }
}