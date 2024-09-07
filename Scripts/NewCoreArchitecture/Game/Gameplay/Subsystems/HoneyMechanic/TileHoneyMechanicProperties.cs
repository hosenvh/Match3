
using Match3.Foundation.Base.ComponentSystem;

namespace Match3.Game.Gameplay.SubSystems.HoneyMechanic
{

    public class TileHoneyMechanicProperties : Component
    {
        public readonly bool canBeTakenOverByHoney;

        public TileHoneyMechanicProperties(bool canBeTakenOverByHoney)
        {
            this.canBeTakenOverByHoney = canBeTakenOverByHoney;
        }
    }
}