using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.HitManagement;

namespace Match3.Game.Gameplay.Tiles
{
    public class IvySackTile : Tile
    {
        private bool isDestroyed;
        
        public IvySackTile(int level) : base(level)
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
            // Maybe looking for a better solution
            if (CurrentLevel() > 0)
                base.InternalHit(hitType, hitCause);
        }

        public bool ShouldGetReadyForGeneration()
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