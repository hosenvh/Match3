

using Match3.Game.Gameplay.Core;
using System.Collections.Generic;

namespace Match3.Game.Gameplay.SubSystems.HintingAndShuffling
{
    public class MatchHint : Hint
    {
        public readonly CellStack origin;
        public readonly CellStack destination;
        public readonly List<CellStack> otherMatchedCellStacks;

        public MatchHint(CellStack origin, CellStack destination, List<CellStack> otherMatchedCellStacks)
        {
            this.origin = origin;
            this.destination = destination;
            this.otherMatchedCellStacks = otherMatchedCellStacks;
        }
    }
}