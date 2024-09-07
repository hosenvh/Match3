

using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.Tiles;
using System.Collections.Generic;

namespace Match3.Game.Gameplay.SubSystemsData.FrameData.ExplosionManagement
{

    public struct DelayedActivationData
    {
        public TileStack tileStack;
        public Tile cause;
        public float delay;

        public DelayedActivationData(TileStack tileStack, Tile cause, float delay)
        {
            this.tileStack = tileStack;
            this.cause = cause;
            this.delay = delay;
        }
    }


    public class InternalExplosionActivationData : BlackBoardData
    {
        public List<DelayedActivationData> delayedTargets = new List<DelayedActivationData>(32);

        public List<TileStack> targets = new List<TileStack>(32);

        public void Clear()
        {
            delayedTargets.Clear();
            targets.Clear();
        }
    }
}