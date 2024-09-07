using Match3.Game.Gameplay.Cells;
using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.Tiles;
using System;
using System.Collections.Generic;
using static Match3.Game.Gameplay.QueryUtilities;

namespace Match3.Game.Gameplay.SubSystems.IvyMechanic
{
    public class IvyValidCellsToGrowUpdater
    {
        HashSet<CellStack> validCellStacksToGrowBush = new HashSet<CellStack>();
        HashSet<CellStack> validCellStacksToGrowRoot = new HashSet<CellStack>();

        CellStack[] allCellStacks;
        CellStackBoard cellStackBoard;

        IvyBoardStateHandler ivyBoardStateHandler;
        BoardBlockageController boardBlockageController;

        public IvyValidCellsToGrowUpdater(GameBoard gameBoard, IvyBoardStateHandler ivyBoardStateHandler, BoardBlockageController boardWallsQueryHandler)
        {
            this.allCellStacks = gameBoard.ArrbitrayCellStackArray();
            this.ivyBoardStateHandler = ivyBoardStateHandler;
            this.cellStackBoard = gameBoard.CellStackBoard();
            this.boardBlockageController = boardWallsQueryHandler;
        }

        public void UpdateValidCellStackForGrowing()
        {
            validCellStacksToGrowBush.Clear();
            validCellStacksToGrowRoot.Clear();

            foreach (var cellStack in ivyBoardStateHandler.IvyRootsCellStacks())
            {
                TryAddAsValidForGrowingBush(cellStack);
                TryExtractCellStacksValidForGrowingRootFrom(cellStack);
            }
        }

        public bool IsCellStackValidForGrowing(CellStack cellStack)
        {
            return validCellStacksToGrowBush.Contains(cellStack) || validCellStacksToGrowRoot.Contains(cellStack);
        }

        public bool IsCellStackValidForGrowingBush(CellStack cellStack)
        {
            return validCellStacksToGrowBush.Contains(cellStack);
        }

        public bool IsCellStackValidForGrowingRoot(CellStack cellStack)
        {
            return validCellStacksToGrowRoot.Contains(cellStack);
        }

        private void TryExtractCellStacksValidForGrowingRootFrom(CellStack cellStack)
        {
            if (HasCellOnTop<IvyRootCell>(cellStack) == false)
                return;

            TryAddAsValidForGrowingRoot(cellStack, Direction.Down);
            TryAddAsValidForGrowingRoot(cellStack, Direction.Up);
            TryAddAsValidForGrowingRoot(cellStack, Direction.Left);
            TryAddAsValidForGrowingRoot(cellStack, Direction.Right);
        }

        private void TryAddAsValidForGrowingBush(CellStack cellStack)
        {
            // TODO: Check wether can be taken over or not.
            if (HasCellOnTop<IvyRootCell>(cellStack) == true
                && HasTileOnTop<IvyBush>(cellStack) == false
                && IsFullyFree(cellStack)
                && CanGrowBushOn(cellStack))
                validCellStacksToGrowBush.Add(cellStack);
        }

        private void TryAddAsValidForGrowingRoot(CellStack rootCellStack, Direction direction)
        {
            if (boardBlockageController.IsBlocked(rootCellStack, direction))
                return;

            var cellStack = cellStackBoard.DirectionalElementOf(rootCellStack.Position(), direction);

            if (cellStack == null 
                || IsFullyFree(cellStack) == false
                || CanGrowRootIn(cellStack) == false)
                return;

            // TODO: Check wether can be taken over or not.
            validCellStacksToGrowRoot.Add(cellStack);
        }

        private bool CanGrowRootIn(CellStack cellStack)
        {
            return cellStack.Top().GetComponent<IvyMechanicCellProperties>().canBeTakenOverByIvy
                 && (HasAnyTile(cellStack) == false || TopTile(cellStack).GetComponent<IvyMechanicTileProperties>().canBeTakenOverByIvy); 
        }

        private bool CanGrowBushOn(CellStack cellStack)
        {
            return HasAnyTile(cellStack) == true 
                && TopTile(cellStack).GetComponent<IvyMechanicTileProperties>().canBeTakenOverByIvy;
        }

        public void RemoveFromValidsForGrowingRoots(CellStack cellStack)
        {
            validCellStacksToGrowRoot.Remove(cellStack);
        }

        internal void RemoveFromValidsForGrowingBushes(CellStack cellStack)
        {
            validCellStacksToGrowBush.Remove(cellStack);
        }
    }
}