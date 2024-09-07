using System.Collections.Generic;
using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.Factories;
using Match3.Game.Gameplay.Tiles;
using UnityEngine;


namespace Match3.Game.Gameplay.Initialization
{
    public class BalloonBoardSpecificConfigFactory : BoardSpecificConfigFactory<BalloonMainTileItemConfig, BalloonBoardSpecificConfig>
    {
        public BalloonBoardSpecificConfigFactory(BoardConfig boardConfig, TileFactory tileFactory, List<BalloonBoardSpecificConfig> dynamicConfigs) : base(boardConfig, tileFactory, dynamicConfigs)
        {
        }

        protected override Tile CreateFrom(BalloonMainTileItemConfig staticConfig, BalloonBoardSpecificConfig dynamicConfig)
        {
            return tileFactory.CreateBalloonMainTile();
        }

        public void CreateTiedTilesFor(BalloonMainTile balloonMainTile, CellStackBoard cellStackBoard)
        {
            Vector2Int mainTilePosition = balloonMainTile.Parent().Parent().Position();
            int mainTileLinearIndex = cellStackBoard.PositionToLinearIndex(mainTilePosition);
            var dynamicConfig = BoardSpecificConfigFor(mainTileLinearIndex);

            foreach (Vector2Int tiedTilePosition in dynamicConfig.GetTiedTilesPositions())
            {
                Vector2Int relationalDirectionFromItsMainBalloonTile = tiedTilePosition - mainTilePosition;
                BalloonTiedTile tiedTile = CreatBalloonTiedTile(cellStackBoard, tiedTilePosition, relationalDirectionFromItsMainBalloonTile);

                balloonMainTile.AddTiedTiles(tiedTile);
            }
        }

        private BalloonTiedTile CreatBalloonTiedTile(CellStackBoard cellStackBoard, Vector2Int position, Vector2Int relationalDirectionFromItsMainBalloonTile)
        {
            TileStack tileStack = cellStackBoard[position].CurrentTileStack();
            if (tileStack == null)
            {
                tileStack = tileFactory.CreateTileStack();
                cellStackBoard[position].SetCurrnetTileStack(tileStack);
                tileStack.SetPosition(position);
            }
            BalloonTiedTile balloonTiedTile = (BalloonTiedTile) tileFactory.CreateBalloonTiedTile(relationalDirectionFromItsMainBalloonTile);
            tileStack.Push(balloonTiedTile);

            return balloonTiedTile;
        }
    }
}