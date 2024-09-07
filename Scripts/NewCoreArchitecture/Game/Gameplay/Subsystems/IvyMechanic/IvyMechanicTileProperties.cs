

using Match3.Foundation.Base.ComponentSystem;

namespace Match3.Game.Gameplay.SubSystems.IvyMechanic
{
    public class IvyMechanicTileProperties : Component
    {
        public readonly bool canBeTakenOverByIvy;

        public IvyMechanicTileProperties(bool canBeTakenOverByIvy)
        {
            this.canBeTakenOverByIvy = canBeTakenOverByIvy;
        }
    }
}