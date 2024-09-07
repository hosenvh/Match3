
using Match3.Foundation.Base.ComponentSystem;
using Match3.Game.Gameplay.Core;

namespace Match3.Game.Gameplay.Matching
{
    public class TileColorComponent : Component
    {
        public readonly TileColor color;

        public TileColorComponent(TileColor color)
        {
            this.color = color;
        }
    }

}