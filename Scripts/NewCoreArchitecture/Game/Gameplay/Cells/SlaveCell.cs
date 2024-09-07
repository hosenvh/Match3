using System;
using Match3.Game.Gameplay.Core;

namespace Match3.Game.Gameplay.Cells
{
    public class SlaveCell : Cell
    {
        private CompositeCell master;

        public SlaveCell(CompositeCell master)
        {
            this.master = master;
            master.AddSalve(this);
        }

        public override void Hit()
        {
            master.Hit();
        }

        public override bool IsDestroyed()
        {
            return master.IsDestroyed();
        }

        public CompositeCell Master()
        {
            return master;
        }

        public override bool CanContainTile()
        {
            return master.CanContainTile();
        }

        protected override bool InteralDoesAcceptDirectHit()
        {
            return master.DoesAcceptHit(HitManagement.HitType.Direct);
        }

        protected override bool InteralDoesAcceptSideHit()
        {
            return master.DoesAcceptHit(HitManagement.HitType.Side);
        }
    }
}