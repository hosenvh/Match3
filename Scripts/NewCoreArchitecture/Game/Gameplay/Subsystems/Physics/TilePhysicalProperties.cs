using Match3.Foundation.Base.ComponentSystem;

namespace Match3.Game.Gameplay.Physics
{
    public class TilePhysicalProperties : Component
    {
        public readonly bool isAffectedByGravity;

        public TilePhysicalProperties(bool isAffectedByGravity)
        {
            this.isAffectedByGravity = isAffectedByGravity;
        }
    }
}