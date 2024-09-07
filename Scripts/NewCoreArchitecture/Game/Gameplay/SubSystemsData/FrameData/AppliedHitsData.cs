

using Match3.Game.Gameplay.Core;
using System.Collections.Generic;

namespace Match3.Game.Gameplay.HitManagement
{
    public class AppliedHitsData : BlackBoardData
    {
        public List<Tile> tilesStartedBeingHit = new List<Tile>(64);
        public List<Tile> tilesFinishedBeingHit = new List<Tile>(64);

        public List<Cell> cellsStartedBeingHit = new List<Cell>(64);

        public void Clear()
        {
            tilesStartedBeingHit.Clear();
            tilesFinishedBeingHit.Clear();
            cellsStartedBeingHit.Clear();
        }
    }
}