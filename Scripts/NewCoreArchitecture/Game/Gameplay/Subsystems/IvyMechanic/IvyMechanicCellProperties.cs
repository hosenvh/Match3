

using Match3.Foundation.Base.ComponentSystem;

namespace Match3.Game.Gameplay.SubSystems.IvyMechanic
{
    public class IvyMechanicCellProperties : Component
    {
        public readonly bool canBeTakenOverByIvy;

        public IvyMechanicCellProperties(bool canBeTakenOverByIvy)
        {
            this.canBeTakenOverByIvy = canBeTakenOverByIvy;
        }
    }
}