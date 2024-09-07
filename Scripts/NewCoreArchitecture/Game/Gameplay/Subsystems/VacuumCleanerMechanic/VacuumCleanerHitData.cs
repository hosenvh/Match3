using Match3.Game.Gameplay.Core;
using System.Collections.Generic;

namespace Match3.Game.Gameplay.SubSystems.VacuumCleanerMechanic
{
    public class VacuumCleanerHitData : BlackBoardData
    {
        public List<CellStack> cellStacks = new List<CellStack>(16);

        public void Clear()
        {
            cellStacks.Clear();
        }
    }
}