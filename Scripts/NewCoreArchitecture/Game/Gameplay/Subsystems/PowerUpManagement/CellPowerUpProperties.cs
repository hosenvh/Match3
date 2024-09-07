

using Match3.Foundation.Base.ComponentSystem;

namespace Match3.Game.Gameplay.SubSystems.PowerUpManagement
{
    public class CellPowerUpProperties : Component
    {

        public readonly bool isHammerTarget;

        public CellPowerUpProperties(bool isHammerTarget)
        {
            this.isHammerTarget = isHammerTarget;
        }
    }
}