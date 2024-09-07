using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.HitManagement;
using Match3.Game.Gameplay.SubSystems.LevelEnding;
using System;

namespace Match3.Game.Gameplay.Tiles
{
    public class ColoredBead : Tile, DestructionBasedGoalObject
    {
        public enum DirtinessState
        {
            Clean,
            Dirty
        };

        public event Action<DirtinessState> onDirtinessChanged = delegate { };

        private DirtinessState dirtinessState;

        public ColoredBead(DirtinessState dirtinessState)
        {
            this.dirtinessState = dirtinessState;
        }

        public void SetDirtinessState(DirtinessState state)
        {
            this.dirtinessState = state;
            onDirtinessChanged(state);
        }

        public DirtinessState GetDirtinessState()
        {
            return dirtinessState;
        }

 
        protected override void InternalHit(HitType hitType, HitCause hitCause)
        {
            if (ShouldBeCleaned(hitType, hitCause))
                SetDirtinessState(DirtinessState.Clean);
            else
                base.InternalHit(hitType, hitCause);
        }

        bool ShouldBeCleaned(HitType hitType, HitCause hitCause)
        {
            return hitType == HitType.Direct && IsDirty() && hitCause is MatchHitCause == false;
        }

        public bool IsClean()
        {
            return dirtinessState == DirtinessState.Clean;
        }

        public bool IsDirty()
        {
            return dirtinessState == DirtinessState.Dirty;
        }
    }
}