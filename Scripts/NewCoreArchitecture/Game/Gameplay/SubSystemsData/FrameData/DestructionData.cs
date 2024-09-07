

using Match3.Game.Gameplay.Core;
using System.Collections.Generic;

namespace Match3.Game.Gameplay
{
    public class DestructionData : BlackBoardData
    {
        public List<Tile> tilesToDestroy = new List<Tile>(64);
        public List<Cell> cellsToDestroy = new List<Cell>(64);
        public List<HitableCellAttachment> cellAttachmentsToDestroy = new List<HitableCellAttachment>(16);

        public void Clear()
        {
            tilesToDestroy.Clear();
            cellsToDestroy.Clear();
            cellAttachmentsToDestroy.Clear();
        }
    }
}