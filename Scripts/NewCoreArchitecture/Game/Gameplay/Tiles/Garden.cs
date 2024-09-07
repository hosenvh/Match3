
using System;
using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.HitManagement;

namespace Match3.Game.Gameplay.Tiles
{
    public class Garden : Tile
    {
        public event Action onReset = delegate { };
        int fillLevel;


        public Garden()
        {
            this.fillLevel = 0;
        }

        protected override void InternalHit(HitType hitType, HitCause hitCause)
        {
            ++fillLevel;
        }

        public int FillLevel()
        {
            return fillLevel;
        }

        public bool IsFilled()
        {
            return fillLevel >= 3;
        }

        public void Reset()
        {
            fillLevel = 0;
            onReset();
        }
    }
}