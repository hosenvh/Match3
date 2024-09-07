using KitchenParadise.Utiltiy.Base;
using Match3.Game.Gameplay.Core;
using System.Collections.Generic;

namespace Match3.Game.Gameplay.Matching
{
    public class Match
    {
        public HashSet<TileStack> tileStacks = new HashSet<TileStack>();

        public Match()
        {

        }

        public Match(Match other)
        {
            tileStacks.UnionWith(other.tileStacks);
        }

        public void Add(TileStack tileStack)
        {
            tileStacks.Add(tileStack);
        }
    }
}