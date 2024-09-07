using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.Tiles;
using System;
using System.Collections.Generic;
using static Match3.Game.Gameplay.QueryUtilities;


namespace Match3.Game.Gameplay.SubSystems.RocketMechanic.TargetFinders
{


    public class BelowLemonadeFinder : TargetFinder
    {
        HashSet<CellStack> cellStackHashset = new HashSet<CellStack>();

        private TypeExclusionChecker typeExclusionChecker;


        public BelowLemonadeFinder(TypeExclusionChecker typeExclusionChecker)
        {
            this.typeExclusionChecker = typeExclusionChecker;
        }

        public void Find(GameBoard gameBoard, ref List<CellStack> targets)
        {
            cellStackHashset.Clear();
            foreach (var cellStack in gameBoard.ArrbitrayCellStackArray())
                if (HasTileOnTop<Lemonade>(cellStack.CurrentTileStack()) && IsFullyFree(cellStack))
                    AddTilesBelow(cellStack, gameBoard);

            targets.AddRange(cellStackHashset);

        }

        private void AddTilesBelow(CellStack cellStack, GameBoard gameBoard)
        {
            var nextCellStack = gameBoard.CellStackBoard().DirectionalElementOf(cellStack.Position(), Direction.Down);

            while(nextCellStack != null)
            {
                if (IsValid(nextCellStack))
                    cellStackHashset.Add(nextCellStack);

                nextCellStack = gameBoard.CellStackBoard().DirectionalElementOf(nextCellStack.Position(), Direction.Down);
            }
        }

        bool IsValid(CellStack cellStack)
        {
            return cellStack != null
                && cellStack.HasTileStack()
                && cellStack.CurrentTileStack().IsDepleted() == false
                && IsFullyFree(cellStack)
                && typeExclusionChecker.IsNotExcluded(cellStack.CurrentTileStack().Top().GetType());
        }
    }
}