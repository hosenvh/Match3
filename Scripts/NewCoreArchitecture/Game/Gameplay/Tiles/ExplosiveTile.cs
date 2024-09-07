
using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.SubSystems.LevelEnding;

namespace Match3.Game.Gameplay.Tiles
{
    public abstract class ExplosiveTile : Tile, DestructionBasedGoalObject
    {
        int maxEffectRadius;
        int effectDistance;

        protected ExplosiveTile(int maxEffectRadius, int effectDistance)
        {
            this.maxEffectRadius = maxEffectRadius;
            this.effectDistance = effectDistance;
        }

        public int MaxEffectRadius()
        {
            return maxEffectRadius;
        }

        public int EffectDistance()
        {
            return effectDistance;
        }

    }
}