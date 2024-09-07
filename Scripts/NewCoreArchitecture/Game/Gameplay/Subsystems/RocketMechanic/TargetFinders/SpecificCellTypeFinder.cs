using Match3.Game.Gameplay.Core;
using System;
using System.Collections.Generic;


namespace Match3.Game.Gameplay.SubSystems.RocketMechanic.TargetFinders
{

    public class SpecificCellTypeFinder : TargetFinder
    {
        Type specifiedCellType;
        TypeExclusionChecker exclusionChecker;

        public SpecificCellTypeFinder(Type cellType, TypeExclusionChecker exclusionChecker)
        {
            this.specifiedCellType = cellType;
            this.exclusionChecker = exclusionChecker;
        }

        public void Find(GameBoard gameBoard, ref List<CellStack> targets)
        {
            foreach (var cellStack in gameBoard.ArrbitrayCellStackArray())
                if (HasSpecifiedCellOnTop(cellStack) && TopTileIsNotExcluded(cellStack) && QueryUtilities.IsFullyFree(cellStack))
                    targets.Add(cellStack);
        }

        private bool TopTileIsNotExcluded(CellStack cellStack)
        {
            if (cellStack.HasTileStack() == false || cellStack.CurrentTileStack().IsDepleted())
                return true;

            return exclusionChecker.IsNotExcluded(cellStack.CurrentTileStack().Top().GetType());
        }

        bool HasSpecifiedCellOnTop(CellStack cellStack)
        {
            return specifiedCellType.IsAssignableFrom(cellStack.Top().GetType());
        }
    }
}