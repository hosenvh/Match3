using Match3.Game.Gameplay.Tiles;
using System.Collections.Generic;
using Match3.Game.Gameplay.Core;


namespace Match3.Game.Gameplay
{
    public struct SingleExplosionHitData
    {
        public readonly List<DelayedCellHitData> hitsData;
        public readonly ExplosiveTile explosiveTile;

        public SingleExplosionHitData(ExplosiveTile explosiveTile, List<DelayedCellHitData> hitsData)
        {
            this.explosiveTile = explosiveTile;
            this.hitsData = hitsData;
        }
    }
    public struct ExplosionProcessingData
    {
        public ExplosiveTile explosiveTile;
        public Tile directCause;
        public readonly int distanceToFirstCause;

        public ExplosionProcessingData(ExplosiveTile explosiveTile, Tile directCause, int distanceToFirstCause)
        {
            this.explosiveTile = explosiveTile;
            this.directCause = directCause;
            this.distanceToFirstCause = distanceToFirstCause;
        }
    }
    public class ExplosionActivationData : BlackBoardData
    {
        public List<SingleExplosionHitData> explosionsHitData = new List<SingleExplosionHitData>(4);

        public List<ExplosionProcessingData> processedExplosives = new List<ExplosionProcessingData>(16);

        public void Clear()
        {
            processedExplosives.Clear();
            explosionsHitData.Clear();
        }
    }
}