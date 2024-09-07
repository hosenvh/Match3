using Match3.Game.Gameplay.Core;
using System.Collections.Generic;

namespace Match3.Game.Gameplay.SubSystemsData.FrameData.RainbowMechanic
{
    public class DirectRainbowActivationRequestData : BlackBoardData
    {
        public List<TileStack> targets = new List<TileStack>(8);


        public void Clear()
        {
            targets.Clear();
        }

    }
}