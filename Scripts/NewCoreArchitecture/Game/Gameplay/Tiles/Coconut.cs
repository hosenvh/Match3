using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.HitManagement;
using Match3.Game.Gameplay.SubSystems.LevelEnding;

namespace Match3.Game.Gameplay.Tiles
{
    public class Coconut : Tile, DestructionBasedGoalObject
    {
        public Coconut(int level) : base(level)
        {
        }

        protected override bool InteralDoesAcceptDirectHit(HitCause hitCause)
        {
            return true;
        }

        protected override bool InteralDoesAcceptSideHit(HitCause hitCause)
        {
            return false;
        }

        protected override void InternalHit(HitType hitType, HitCause hitCause)
        {
            if (CurrentLevel() > 0)
                base.InternalHit(hitType, hitCause);
        }

        public bool ShouldGetReadyForDestroy()
        {
            return CurrentLevel() <= 0;
        }
    }
}