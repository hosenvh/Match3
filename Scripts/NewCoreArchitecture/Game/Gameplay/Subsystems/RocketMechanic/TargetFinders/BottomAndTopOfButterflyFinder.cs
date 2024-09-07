using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.Tiles;
using System.Collections.Generic;
using static Match3.Game.Gameplay.QueryUtilities;


namespace Match3.Game.Gameplay.SubSystems.RocketMechanic.TargetFinders
{
    public class BottomAndTopOfButterflyFinder : TargetFinder
    {
        HashSet<CellStack> cellStackHashset = new HashSet<CellStack>();
        private TypeExclusionChecker typeExclusionChecker;

        public BottomAndTopOfButterflyFinder(TypeExclusionChecker typeExclusionChecker)
        {
            this.typeExclusionChecker = typeExclusionChecker;
        }

        public void Find(GameBoard gameBoard, ref List<CellStack> targets)
        {
            cellStackHashset.Clear();
            foreach (var cellStack in gameBoard.ArrbitrayCellStackArray())
                if (HasTileOnTop<Butterfly>(cellStack.CurrentTileStack()) && IsFullyFree(cellStack))
                    AddTilesBottomAndTopOf(cellStack, gameBoard);

            targets.AddRange(cellStackHashset);

        }

        private void AddTilesBottomAndTopOf(CellStack cellStack, GameBoard gameBoard)
        {
            var bottomCellStack = gameBoard.CellStackBoard().DirectionalElementOf(cellStack.Position(), Direction.Down);
            var topCellStack = gameBoard.CellStackBoard().DirectionalElementOf(cellStack.Position(), Direction.Up);

            if (IsValid(bottomCellStack))
                    cellStackHashset.Add(bottomCellStack);

            if (IsValid(topCellStack))
                cellStackHashset.Add(topCellStack);
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