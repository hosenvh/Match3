using System.Collections.Generic;
using Match3.Game.Gameplay.Cells;

namespace Match3.Game.Gameplay.SubSystemsData.FrameData.RiverMechanic
{
    public class RiverBlockingData : BlackBoardData
    {
        public List<RiverCell> BlockedRiverCells = new List<RiverCell>();
        
        public void Clear()
        {
            BlockedRiverCells.Clear();
        }
    }
}