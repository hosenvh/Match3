using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.HitManagement;

namespace Match3.Game.Gameplay.Tiles
{
    public class Louie : Tile
    {
        private bool isDestroyed;

        public Louie(int level) : base(level)
        {
        }

        protected override bool InteralDoesAcceptDirectHit(HitCause hitCause)
        {
            return true;
        }

        protected override bool InteralDoesAcceptSideHit(HitCause hitCause)
        {
            return true;
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

        public void MarkAsDestroyed()
        {
            isDestroyed = true;
        }

        public override bool IsDestroyed()
        {
            return isDestroyed;
        }
    }
}