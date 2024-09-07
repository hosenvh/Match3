

using Match3.Game.Gameplay.Core;
using System.Collections.Generic;

namespace Match3.Game.Gameplay
{
    public class CellStacksToHitData : BlackBoardData
    {
        public List<CellStack> cells = new List<CellStack>();

        public void Clear()
        {
            cells.Clear();
        }
    }
}