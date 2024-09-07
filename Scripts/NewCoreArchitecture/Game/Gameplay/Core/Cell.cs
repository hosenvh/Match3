using System;
using Match3.Foundation.Base.ComponentSystem;
using Match3.Game.Gameplay.HitManagement;

namespace Match3.Game.Gameplay.Core
{
    // TODO: Try to move IsDestroyed and/or Hit to another class. For many cells they are degenerate.
    public abstract class Cell : BasicEntity
    {
        CellStack parent;


        public abstract bool CanContainTile();


        public bool DoesAcceptHit(HitType hitType)
        {
            switch (hitType)
            {
                case HitType.Direct:
                    return InteralDoesAcceptDirectHit();
                case HitType.Side:
                    return InteralDoesAcceptSideHit();
            }

            return false;
        }

        protected abstract bool InteralDoesAcceptDirectHit();
        protected abstract bool InteralDoesAcceptSideHit();

        public abstract void Hit();

        public abstract bool IsDestroyed();

        public void SetParent(CellStack parent)
        {
            this.parent = parent;
        }

        public CellStack Parent()
        {
            return this.parent;
        }
    }
}