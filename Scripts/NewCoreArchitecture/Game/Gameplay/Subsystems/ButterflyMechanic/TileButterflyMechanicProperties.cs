using Match3.Foundation.Base.ComponentSystem;

namespace Match3.Game.Gameplay.SubSystems.ButterflyMechanic
{
    public struct TileButterflyMechanicProperties : Component
    {
        public readonly bool canButterflyBeGeneratedOn;

        public TileButterflyMechanicProperties(bool canButterflyBeGeneratedOn)
        {
            this.canButterflyBeGeneratedOn = canButterflyBeGeneratedOn;
        }
    }
}