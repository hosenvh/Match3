using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.HitManagement;

namespace Match3.Game.Gameplay.Tiles
{
    public class CatColoredBead : Tile
    {
        protected override bool InteralDoesAcceptDirectHit(HitCause hitCause)
        {
            return true;
        }

        protected override bool InteralDoesAcceptSideHit(HitCause hitCause)
        {
            return true;
        }
    }
}