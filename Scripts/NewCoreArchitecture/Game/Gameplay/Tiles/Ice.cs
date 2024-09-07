

using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.SubSystems.LevelEnding;


namespace Match3.Game.Gameplay.Tiles
{
    public class Ice : Tile , DestructionBasedGoalObject
    {
        public Ice(int level) : base(level)
        {
        }
    }
}