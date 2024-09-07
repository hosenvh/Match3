

using Match3.Game.Gameplay.Core;
using System.Collections.Generic;

namespace Match3.Game.Gameplay
{
    public class DestroyedObjectsData : BlackBoardData
    {
        public List<Tile> tiles = new List<Tile>(64);
        public List<Cell> cells = new List<Cell>(64);
        public List<HitableCellAttachment> attachments = new List<HitableCellAttachment>(64);


        public void Clear()
        {
            cells.Clear();
            tiles.Clear();
            attachments.Clear();
        }
    }
}