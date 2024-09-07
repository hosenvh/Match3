using System.Collections.Generic;


namespace Match3.Game.Gameplay.Swapping
{
    public class RequestedSwapsData : BlackBoardData
    {
        public List<SwapRequestData> data = new List<SwapRequestData>(4);

        public void Clear()
        {
            data.Clear();
        }
    }
}