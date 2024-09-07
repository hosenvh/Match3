using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.HitManagement;


namespace Match3.Game.Gameplay.Tiles
{
    public class ChickenNest : Tile
    {
        private const int NEEDED_HITS_TO_GET_DESTROY_COUNT = 3;

        private bool isDestroyed;

        public ChickenNest() : base(initialLevel: NEEDED_HITS_TO_GET_DESTROY_COUNT)
        {
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