
using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.HitManagement;

namespace Match3.Game.Gameplay.Tiles
{
    public class Honeycomb : Tile
    {
        public Honeycomb() : base(1)
        {
        }

        protected override bool InteralDoesAcceptDirectHit(HitCause hitCause)
        {
            return false;
        }

        protected override bool InteralDoesAcceptSideHit(HitCause hitCause)
        {
            return false;
        }
    }
}