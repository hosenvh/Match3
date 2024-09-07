

using Match3.Game.Gameplay.Core;

namespace Match3.Game.Gameplay
{
    public struct DelayedCellHitData
    {
        public readonly CellStack cellStack;
        public readonly float delay;

        public DelayedCellHitData(CellStack cellStack, float delay)
        {
            this.cellStack = cellStack;
            this.delay = delay;
        }
    }
}