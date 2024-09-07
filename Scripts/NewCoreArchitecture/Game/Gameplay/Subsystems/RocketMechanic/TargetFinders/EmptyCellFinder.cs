using Match3.Game.Gameplay.Core;
using System.Collections.Generic;


namespace Match3.Game.Gameplay.SubSystems.RocketMechanic.TargetFinders
{
    public class EmptyCellFinder : TargetFinder
    {
        public void Find(GameBoard gameBoard, ref List<CellStack> targets)
        {
            foreach (var cellStack in gameBoard.ArrbitrayCellStackArray())
                if ((cellStack.HasTileStack() == false || cellStack.CurrentTileStack().IsDepleted())
                    && cellStack.Top().CanContainTile())
                    targets.Add(cellStack);

        }

    }
}