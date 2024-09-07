using System;
using Match3.Game.Gameplay.Core;

namespace Match3.Game.Gameplay.Cells
{
    public class GrassCell : Cell
    {
        public event Action onLevelIncreased = delegate { };

        readonly int maxLevel;
        int level;

        public GrassCell(int level, int maxLevel)
        {
            this.maxLevel = maxLevel;
            this.level = Math.Min(level, maxLevel);
        }

        public override void Hit()
        {
            --level;
        }

        public override bool IsDestroyed()
        {
            return level <= 0;
        }

        public int CurrentLevel()
        {
            return level;
        }

        public override bool CanContainTile()
        {
            return true;
        }

        protected override bool InteralDoesAcceptDirectHit()
        {
            return true;
        }

        protected override bool InteralDoesAcceptSideHit()
        {
            return false;
        }

        public void IncreaseLevel(int levelUpAmount)
        {
            this.level = Math.Min(level + levelUpAmount, maxLevel);
            onLevelIncreased();
        }
    }
}