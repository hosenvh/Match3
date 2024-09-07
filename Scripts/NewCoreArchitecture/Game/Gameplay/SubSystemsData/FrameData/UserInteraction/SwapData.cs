using Match3.Game.Gameplay.Core;

namespace Match3.Game.Gameplay.Swapping
{
    public struct SwapRequestData
    {
        public TileStack target;
        public Direction direction;

        public SwapRequestData(TileStack target, Direction direction)
        {
            this.target = target;
            this.direction = direction;
        }
    }

    public struct SwapExecutionData
    {
        public readonly CellStack originTarget;
        public readonly CellStack destinationTarget;

        public SwapExecutionData(CellStack origin, CellStack destination)
        {
            this.originTarget = origin;
            this.destinationTarget = destination;
        }
    }
}