

using Match3.Game.Gameplay.Core;

namespace Match3.Game.Gameplay.SubSystems.HintingAndShuffling
{
    public class MoveHint : Hint
    {
        public readonly CellStack origin;
        public readonly CellStack destination;

        public MoveHint(CellStack origin, CellStack destination)
        {
            this.origin = origin;
            this.destination = destination;
        }
    }
}