

using Match3.Game.Gameplay.Core;

namespace Match3.Game.Gameplay.Tiles
{
    public class ExtraMove : Tile
    {
        public readonly int moveAmount;

        public ExtraMove(int moveAmount)
        {
            this.moveAmount = moveAmount;
        }
    }
}