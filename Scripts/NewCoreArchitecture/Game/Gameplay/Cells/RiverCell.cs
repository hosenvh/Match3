

using System;
using Match3.Game.Gameplay.CellAttachments;
using Match3.Game.Gameplay.Core;

namespace Match3.Game.Gameplay.Cells
{
    public class RiverCell : Cell
    {
        RiverCell nextRiverCell;

        public override void Hit()
        {

        }

        public override bool IsDestroyed()
        {
            return false;
        }

        public override bool CanContainTile()
        {
            if (Parent().HasAttachment<LilyPad>())
                return true;

            var lilyPadBud = Parent().GetAttachment<LilyPadBud>();

            return lilyPadBud != null && lilyPadBud.IsFullyGrown();
        }

        public void SetNextRiverCell(RiverCell riverCell)
        {
            nextRiverCell = riverCell;
        }

        public RiverCell NextRiverCell()
        {
            return nextRiverCell;
        }

        protected override bool InteralDoesAcceptDirectHit()
        {
            return true;
        }

        protected override bool InteralDoesAcceptSideHit()
        {
            return true;
        }
    }
}