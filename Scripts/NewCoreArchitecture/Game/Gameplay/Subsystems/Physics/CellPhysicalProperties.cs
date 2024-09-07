
using Match3.Foundation.Base.ComponentSystem;

namespace Match3.Game.Gameplay.Physics
{
    public class CellPhysicalProperties : Component
    {
        public readonly bool allowTileFallThrough;

        public CellPhysicalProperties(bool canTileFallThrough)
        {
            this.allowTileFallThrough = canTileFallThrough;
        }
    }
}