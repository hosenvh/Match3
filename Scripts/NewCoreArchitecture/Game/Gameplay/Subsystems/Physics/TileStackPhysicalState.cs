
using Match3.Foundation.Base.ComponentSystem;
using System;

namespace Match3.Game.Gameplay.Physics
{
    public enum PhysicsState { Falling, Resting };
    public class TileStackPhysicalState : Component
    {
        public event Action<PhysicsState> onPhysicalStateChanged = delegate { };


        public bool hasReachedTarget;
        public float speed;


        private PhysicsState physicsState = Physics.PhysicsState.Resting;

        public void SetPhysicsState(PhysicsState state)
        {
            if (this.physicsState != state)
            {
                this.physicsState = state;
                onPhysicalStateChanged(state);
            }
        }

        public PhysicsState PhysicsState()
        {
            return physicsState;
        }
    }
}