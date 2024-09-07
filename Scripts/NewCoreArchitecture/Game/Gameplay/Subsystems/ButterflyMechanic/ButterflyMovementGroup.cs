using Match3.Game.Gameplay.Core;
using System.Collections.Generic;

namespace Match3.Game.Gameplay.SubSystems.ButterflyMechanic
{
    public struct ButterflyMovementGroup
    {
        public readonly List<TileStack> butterfliesTilestacks;
        public TileStack topTileStack;

        public ButterflyMovementGroup(List<TileStack> butterfliesTilestacks)
        {
            this.butterfliesTilestacks = butterfliesTilestacks;
            topTileStack = null;
        }

        public void Add(TileStack tileStack)
        {
            butterfliesTilestacks.Add(tileStack);
        }

        public TileStack Head()
        {
            return butterfliesTilestacks[butterfliesTilestacks.Count - 1];
        }

        public TileStack Tail()
        {
            return butterfliesTilestacks[0];
        }
    }
}