using Match3.Game.Gameplay.Core;
using System.Collections.Generic;

namespace Match3.Game.Gameplay
{
    public class PowerUpActivationData : BlackBoardData
    {
        public List<CellStack> affectedCellStacks = new List<CellStack>();

        public void Clear()
        {
            affectedCellStacks.Clear();
        }
    }
}