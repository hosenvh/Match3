using System;
using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.HitManagement;


namespace Match3.Game.Gameplay.Tiles
{
    public class IceMakerMainTile : CompositeTile
    {
        public Action onHit = delegate { };
        private int remainedIce;
        private bool isDestroyed;


        public IceMakerMainTile() : base(new Size(2, 2))
        {
            remainedIce = 9;
        }

        protected override void InternalHit(HitType hitType, HitCause hitCause)
        {
            onHit.Invoke();
        }

        public void SetAsDestroyed()
        {
            isDestroyed = false;
        }

        public void PopIce(int count)
        {
            remainedIce -= count;
        }

        public int GetRemainedIceCount()
        {
            return remainedIce;
        }

        public override bool IsDestroyed() // It means it can be destroyed or not
        {
            return isDestroyed;
        }

        public bool IsEmpty()
        {
            return remainedIce <= 0;
        }
    }
}
