using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.SubSystems.LevelEnding;

namespace Match3.Game.Gameplay.Tiles
{
    public class Nut : Tile, DestructionBasedGoalObject
    {
        public Nut(int initialLevel) : base(initialLevel)
        {
        }
    }
}