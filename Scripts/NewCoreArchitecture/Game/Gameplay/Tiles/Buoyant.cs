using System;
using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.HitManagement;
using Match3.Game.Gameplay.SubSystems.LevelEnding;

namespace Match3.Game.Gameplay.Tiles
{
    public class Buoyant : Tile, DestructionBasedGoalObject
    {
        private readonly TileColor color;

        // TODO: Maybe move this to a separate component?
        int turnsWaited = 0;

        public Buoyant(TileColor color)
        {
            this.color = color;
        }

        public void IncrementWaitingTurns()
        {
            ++turnsWaited;
        }

        public int TurnsWaited()
        {
            return turnsWaited;
        }

        public void ResetWaiting()
        {
            turnsWaited = 0;
        }

        protected override bool InteralDoesAcceptSideHit(HitCause hitCause)
        {
            switch(hitCause)
            {
                case MatchHitCause matchHitCause:
                    return matchHitCause.matchColor == this.color;
                case RainbowHitCause rainbowHitCause:
                    return rainbowHitCause.targetColor == this.color;
                default:
                    return false;

            }
        }

        public TileColor Color()
        {
            return color;
        }
    }
}