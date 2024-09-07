using Match3.Game.Gameplay.Core;
using System.Collections.Generic;


namespace Match3.Game.Gameplay.SubSystems.RocketMechanic
{
    public interface TargetFinder
    {
        void Find(GameBoard gameBoard, ref List<CellStack> targets);
    }
}