
using Match3.Foundation.Base.ComponentSystem;
using Match3.Game.Gameplay.Core;

namespace Match3.Game.Gameplay.Matching
{
    public class ColorProperty : Component
    {
        public readonly TileColor tileColor;

        public ColorProperty(TileColor tileColor)
        {
            this.tileColor = tileColor;
        }
    }
}