
using Match3.Foundation.Base.ComponentSystem;

namespace Match3.Game.Gameplay.Matching
{
    public class TileMatchingProperties : Component
    {
        public readonly bool allowsMatchFallThrough;

        public TileMatchingProperties(bool allowsMatchFallThrough)
        {
            this.allowsMatchFallThrough = allowsMatchFallThrough;
        }
    }
}