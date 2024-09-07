using System;
using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.HitManagement;
using Match3.Game.Gameplay.SubSystems.LevelEnding;

namespace Match3.Game.Gameplay.Tiles
{
    public class JamJar : Tile, DestructionBasedGoalObject
    {
        const int INITIAL_LEVEL = 2;

        private TileColor filledColor = TileColor.None;

        public JamJar() : base(initialLevel: INITIAL_LEVEL)
        {
        }

        public TileColor FilledColor()
        {
            return filledColor;
        }

        protected override bool InteralDoesAcceptDirectHit(HitCause hitCause)
        {
            if (IsFilled())
                return true;
            else
                return false;
               
        }

        protected override bool InteralDoesAcceptSideHit(HitCause hitCause)
        {
            var hitColor = ColorOf(hitCause);

            if (hitColor == TileColor.None)
                return false;

            if (IsFilled())
                return hitColor == filledColor;
            else
                return true;
        }

        private TileColor ColorOf(HitCause hitCause)
        {
            switch (hitCause)
            {
                case MatchHitCause matchHitCause:
                    return matchHitCause.matchColor;
                case RainbowHitCause rainbowHitCause:
                    return rainbowHitCause.targetColor;
            }

            return TileColor.None;

        }

        protected override void InternalHit(HitType hitType, HitCause hitCause)
        {
            if (hitType == HitType.Side && IsFilled() == false)
                filledColor = ColorOf(hitCause);

            DecrementLevel();
        }

        private bool IsFilled()
        {
            return CurrentLevel() < INITIAL_LEVEL;
        }
    }
}