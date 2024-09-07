using Match3.Game.Gameplay.Core;

namespace Match3.Game.Gameplay.Cells
{
    public class GroundCell : Cell
    {
        public override void Hit()
        {
            
        }

        public override bool IsDestroyed()
        {
            return false;
        }

        public override bool CanContainTile()
        {
            return true;
        }

        protected override bool InteralDoesAcceptDirectHit()
        {
            return false;
        }

        protected override bool InteralDoesAcceptSideHit()
        {
            return false;
        }
    }
}