using System;
using System.Collections.Generic;
using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.Factories;
using Match3.Game.Gameplay.SubSystems.DuckMechanic;

namespace Match3.Game.Gameplay.Initialization
{
    public class DuckBoardSpecificConfigFactory
    {
        protected BoardConfig boardConfig;
        protected TileFactory tileFactory;

        public DuckBoardSpecificConfigFactory (BoardConfig boardConfig, TileFactory tileFactory)
        {
            this.boardConfig = boardConfig;
            this.tileFactory = tileFactory;
        }

        public Tile CreateDuckTile ()
        {
            DuckSystem.SetChildTilesTypes(boardConfig.levelConfig.duckItemData.childTilesTypes);
            return tileFactory.CreateDuckTile(DuckSystem.childTilesTypes);
        }
    }
}