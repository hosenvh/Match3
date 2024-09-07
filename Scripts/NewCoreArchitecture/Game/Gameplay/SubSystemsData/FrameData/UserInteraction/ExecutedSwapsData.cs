using System.Collections.Generic;
using Match3.Foundation.Base.ComponentSystem;

namespace Match3.Game.Gameplay.Swapping
{
    public class  ExecutedSwapsData : BlackBoardData
    {
        public List<SwapExecutionData> data = new List<SwapExecutionData>(4);

        public void Clear()
        {
            data.Clear();
        }
    }
}