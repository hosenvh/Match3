using System.Collections.Generic;
using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.Factories;

namespace Match3.Game.Gameplay.Initialization
{
    public class TableClothBoardSpecificConfigFactory : BoardSpecificConfigFactory<TableClothConfig, TableClothBoardSpecificConfig>
    {
        public TableClothBoardSpecificConfigFactory(BoardConfig boardConfig, TileFactory tileFactory, List<TableClothBoardSpecificConfig> dynamicConfigs) : base(boardConfig, tileFactory, dynamicConfigs)
        {
        }

        protected override Tile CreateFrom(TableClothConfig staticConfig, TableClothBoardSpecificConfig dynamicConfig)
        {
            return tileFactory.CreateTableClothMainTile(
                new Size(dynamicConfig.size.x, dynamicConfig.size.y), 
                dynamicConfig.firstTargetData.ToLogical(), 
                dynamicConfig.secondTargetData.ToLogical());
        }
    }

}