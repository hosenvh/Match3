
using Match3.Game.Gameplay.Swapping;
using System.Collections.Generic;

namespace Match3.Game.Gameplay.Input
{
    public class SuccessfulSwapsData : BlackBoardData
    {
        public List<SwapExecutionData> data = new List<SwapExecutionData>(4);

        public void Clear()
        {
            data.Clear();
        }
    }
}