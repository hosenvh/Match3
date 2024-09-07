using System.Collections.Generic;
using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.Factories;
using Match3.Game.Gameplay.Tiles;

namespace Match3.Game.Gameplay.Initialization
{
    public class GrassSackBoardSpecificConfigFactory : BoardSpecificConfigFactory<GrassSackConfig, GrassSackBoardSpecificConfig>
    {
        public GrassSackBoardSpecificConfigFactory(BoardConfig boardConfig, TileFactory tileFactory, List<GrassSackBoardSpecificConfig> dynamicConfigs) : base(boardConfig, tileFactory, dynamicConfigs)
        {
        }

        protected override Tile CreateFrom(GrassSackConfig staticConfig, GrassSackBoardSpecificConfig dynamicConfig)
        {
            var grassSack = tileFactory.CreateGrassSack() as GrassSackMainTile;

            foreach (var data in dynamicConfig.artifactsData)
                grassSack.AddArtifactData(data);

            return grassSack;

        }
    }

}