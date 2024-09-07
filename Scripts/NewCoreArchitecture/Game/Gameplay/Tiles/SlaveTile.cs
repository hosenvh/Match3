
using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.HitManagement;

namespace Match3.Game.Gameplay.Tiles
{
    public class SlaveTile : Tile
    {
        private CompositeTile master;

        public SlaveTile(CompositeTile master)
        {
            this.master = master;
            master.AddSalve(this);
        }

        public CompositeTile Master()
        {
            return master;
        }

        protected override bool InteralDoesAcceptDirectHit(HitCause hitCause)
        {
            return master.DoesAcceptHit(HitType.Direct, hitCause);
        }

        protected override bool InteralDoesAcceptSideHit(HitCause hitCause)
        {
            return master.DoesAcceptHit(HitType.Side, hitCause);
        }

        protected override void InternalHit(HitType hitType, HitCause hitCause)
        {
            master.Hit(hitType, hitCause);
        }

        public override bool IsDestroyed()
        {
            return master.IsDestroyed();
        }
    }
}