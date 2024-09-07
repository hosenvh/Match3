using System;
using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.SubSystems.LevelEnding;

namespace Match3.Game.Gameplay.Cells
{
    public class HedgeCell : Cell, DestructionBasedGoalObject
    {
        int level;

        public HedgeCell(int level)
        {
            this.level = level;
        }

        public override bool CanContainTile()
        {
            return false;
        }

        public override void Hit()
        {
            --level;
        }

        public int CurrentLevel()
        {
            return level;
        }

        public override bool IsDestroyed()
        {
            return level <= 0;
        }

        protected override bool InteralDoesAcceptDirectHit()
        {
            return true;
        }

        protected override bool InteralDoesAcceptSideHit()
        {
            return true;
        }
    }
}