using Match3.Game.Gameplay.Core;
using System.Collections.Generic;


namespace Match3.Game.Gameplay.SubSystemsData.FrameData.RocketMechanic
{
    public class RocketHitData : BlackBoardData
    {
        public List<CellStack> cellStacks = new List<CellStack>(8);

        public void Clear()
        {
            cellStacks.Clear();
        }
    }
}