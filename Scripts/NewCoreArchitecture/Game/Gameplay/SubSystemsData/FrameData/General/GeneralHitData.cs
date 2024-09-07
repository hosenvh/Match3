

using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.HitManagement;
using System.Collections.Generic;

namespace Match3.Game.Gameplay.SubSystemsData.FrameData.General
{
    public class GeneralHitData : BlackBoardData
    {

        public List<CellStack> cellStacksToHit = new List<CellStack>(8);

        public void Clear()
        {
            cellStacksToHit.Clear();
        }
    }
}