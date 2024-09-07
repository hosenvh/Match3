

using Match3.Foundation.Base.ComponentSystem;

namespace Match3.Game.Gameplay.SubSystems.GrassSackMechanic
{
    public class GrassSackMechanicCellProperties : Component
    {
        public readonly bool canCreateItemOnCell;

        public GrassSackMechanicCellProperties(bool canCreateItemOnCell)
        {
            this.canCreateItemOnCell = canCreateItemOnCell;
        }
    }
}