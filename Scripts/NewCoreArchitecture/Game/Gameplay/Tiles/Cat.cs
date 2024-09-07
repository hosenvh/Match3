using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.HitManagement;

namespace Match3.Game.Gameplay.Tiles
{
    public class Cat : Tile
    {

        protected override bool InteralDoesAcceptDirectHit(HitCause hitCause)
        {
            return false;
        }

        protected override bool InteralDoesAcceptSideHit(HitCause hitCause)
        {
            return false;
        }

        protected override void InternalHit(HitType hitType, HitCause hitCause)
        {
            
        }

        public override bool IsDestroyed()
        {
            return false;
        }
    }
}