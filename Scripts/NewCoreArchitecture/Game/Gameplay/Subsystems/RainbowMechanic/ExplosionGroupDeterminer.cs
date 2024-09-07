using System.Collections.Generic;
using Match3.Game.Gameplay.Core;


namespace Match3.Game.Gameplay.RainbowMechanic
{
    public class ExplosionGroup
    {
        public readonly Tile rootCause;
        public readonly HashSet<Tile> tiles = new HashSet<Tile>();

        public ExplosionGroup(Tile rootCause)
        {
            this.rootCause = rootCause;
        }
    }

    public class ExplosionGroupDeterminer
    {
        Dictionary<Tile, Tile> tileToDirectCauseMapping = new Dictionary<Tile, Tile>(5);
        List<ExplosionGroup> groups = new List<ExplosionGroup>(5);

        public void Determine(List<ExplosionProcessingData> processedExplosives)
        {
            tileToDirectCauseMapping.Clear();
            groups.Clear();

            foreach (var explosiveData in processedExplosives)
                RegisterDirectCauseFor(explosiveData.explosiveTile, explosiveData.directCause);

            foreach (var explosiveData in processedExplosives)
                AddTo(group: FindOrCreateAGroupFor(explosiveData.explosiveTile), explosiveData.explosiveTile);
        }

        private void AddTo(ExplosionGroup group, Tile tile)
        {
            group.tiles.Add(tile);

            if (HasDirectCause(tile))
                AddTo(group, GetDirectCauseOf(tile));
        }


        private ExplosionGroup FindOrCreateAGroupFor(Tile tile)
        {
            foreach (var group in groups)
                if (group.tiles.Contains(tile))
                    return group;

            if (HasDirectCause(tile) == false)
            {
                var group = new ExplosionGroup(tile);
                groups.Add(group);
                return group;
            }
            else
                return FindOrCreateAGroupFor(GetDirectCauseOf(tile));
        }

        public List<ExplosionGroup> Groups()
        {
            return groups;
        }

        private void RegisterDirectCauseFor(Tile tile, Tile directCause)
        {
            tileToDirectCauseMapping[tile] = directCause;
        }

        private bool HasDirectCause(Tile tile)
        {
            return tileToDirectCauseMapping.ContainsKey(tile) && tileToDirectCauseMapping[tile] != null;
        }

        private Tile GetDirectCauseOf(Tile tile)
        {
            return tileToDirectCauseMapping[tile];
        }
    }
}