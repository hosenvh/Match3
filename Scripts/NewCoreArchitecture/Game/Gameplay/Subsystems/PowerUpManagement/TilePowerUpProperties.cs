
using Match3.Foundation.Base.ComponentSystem;

namespace Match3.Game.Gameplay.SubSystems.PowerUpManagement
{
    public class TilePowerUpProperties :  Component
    {

        public readonly bool isHammerTarget;

        public TilePowerUpProperties(bool isHammerTarget)
        {
            this.isHammerTarget = isHammerTarget;
        }
    }
}