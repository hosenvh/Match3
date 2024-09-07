

using Match3.Game.Gameplay.Core;

namespace Match3.Game.Gameplay.SubSystems.HintingAndShuffling
{
    public class ActivationHint : Hint
    {
        public readonly CellStack target;

        public ActivationHint(CellStack target)
        {
            this.target = target;
        }
    }

}