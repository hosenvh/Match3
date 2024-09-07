using Match3.Game.Gameplay.Core;

namespace Match3.Game.Gameplay.Cells
{
    // TODO: Rename this.
    public class EmptyCell : Cell
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
            return false;
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