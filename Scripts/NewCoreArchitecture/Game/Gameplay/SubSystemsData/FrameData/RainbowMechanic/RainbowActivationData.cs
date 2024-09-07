

using Match3.Game.Gameplay.Core;
using System.Collections.Generic;
using Match3.Game.Gameplay.Tiles;


namespace Match3.Game.Gameplay.SubSystemsData.FrameData.RainbowMechanic
{
    public class RainbowActivationData : BlackBoardData
    {
        public List<TileStack> tileStackHits = new List<TileStack>(64);
        public List<DelayedCellHitData> delayedCellStackHits = new List<DelayedCellHitData>(32);
        public List<CellStack> activatedRainbowCellStacks = new List<CellStack>(4);
        public HashSet<Rainbow> activeRainbowes = new HashSet<Rainbow>();

        public void Clear()
        {
            tileStackHits.Clear();
            delayedCellStackHits.Clear();
            activatedRainbowCellStacks.Clear();
            activeRainbowes.Clear();
        }
    }
}