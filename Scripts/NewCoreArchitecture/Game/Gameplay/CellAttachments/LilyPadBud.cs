

using Match3.Foundation.Base.ComponentSystem;
using Match3.Game.Gameplay.Core;
using System;

namespace Match3.Game.Gameplay.CellAttachments
{
    // TODO: Consider changing this to Cell or HitableCellAttachment
    public class LilyPadBud : CellAttachment
    {
        public event Action<int> onGrown;
        public event Action<int> onShrunk;

        int growthLevel = 0;


        public LilyPadBud(int growthLevel)
        {
            this.growthLevel = growthLevel;
        }

        public void Grow()
        {
            SetGrowthLevel(growthLevel + 1);
            onGrown(growthLevel);
        }

        public void Shrink()
        {
            SetGrowthLevel(growthLevel - 1);
            onShrunk(growthLevel);
        }

        public bool IsFullyGrown()
        {
            return growthLevel == 2;
        }

        public bool IsFullyShrunk()
        {
            return growthLevel == 0;
        }

        private void SetGrowthLevel(int level)
        {
            this.growthLevel = level;
        }

        public int GrowthLevel()
        {
            return growthLevel;
        }
    }
}