using Match3.Game.Gameplay.Core;
using System;

namespace Match3.Game.Gameplay.Cells
{
    public class CatPathCell : Cell
    {
        CatPathCell nextCell;

        public override bool CanContainTile()
        {
            return true;
        }

        public override void Hit()
        {
        }

        public override bool IsDestroyed()
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

        public void SetNextCell(CatPathCell nextCell)
        {
            this.nextCell = nextCell;
        }

        public CatPathCell NextCell()
        {
            return nextCell;
        }
    }
}