using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.HitManagement;
using Match3.Game.Gameplay.SubSystems.LevelEnding;

namespace Match3.Game.Gameplay.Tiles
{
    public class ColoredBox : Tile, DestructionBasedGoalObject
    {
        public readonly TileColor color;

        public ColoredBox(int level, TileColor color) : base(level)
        {
            this.color = color;
        }

        protected override bool InteralDoesAcceptSideHit(HitCause hitCause)
        {
            return (hitCause is MatchHitCause matchHitCause && matchHitCause.matchColor == this.color)
                || (hitCause is RainbowHitCause rainbowHitCause && rainbowHitCause.targetColor == this.color);
        }
    }
}