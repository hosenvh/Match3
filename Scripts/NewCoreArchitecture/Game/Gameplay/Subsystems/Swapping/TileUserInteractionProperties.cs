
using Match3.Foundation.Base.ComponentSystem;

namespace Match3.Game.Gameplay.Swapping
{
    public class TileUserInteractionProperties : Component
    {
        public readonly bool isSwappable;

        public TileUserInteractionProperties(bool isSwappable)
        {
            this.isSwappable = isSwappable;
        }
    }
}