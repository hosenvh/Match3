using System;
using Match3.Game.Gameplay.Core;

namespace Match3.Game.Gameplay.CellAttachments
{
    public class Rope : HitableCellAttachment
    {
        public readonly GridPlacement placement;

        int level;

        public Rope(GridPlacement placement, int initialLevel)
        {
            this.placement = placement;
            this.level = initialLevel;
        }

        public int CurrentLevel()
        {
            return level;
        }

        public override void Hit()
        {
            --level;
        }

        public override bool IsDestroyed()
        {
            return level <= 0;
        }
    }
}