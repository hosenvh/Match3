using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.SubSystems.LevelEnding;

namespace Match3.Game.Gameplay.Cells
{
    public class IvyRootCell : Cell , DestructionBasedGoalObject
    {
        bool isDestroyed = false;

        public override bool CanContainTile()
        {
            return true;
        }

        public override void Hit()
        {
            isDestroyed = true;
        }

        public override bool IsDestroyed()
        {
            return isDestroyed;
        }

        protected override bool InteralDoesAcceptDirectHit()
        {
            return true;
        }

        protected override bool InteralDoesAcceptSideHit()
        {
            return false;
        }
    }
}