using Match3.Game.Gameplay.Cells;
using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.Tiles;
using System;
using System.Collections.Generic;
using static Match3.Game.Gameplay.QueryUtilities;

namespace Match3.Game.Gameplay.SubSystems.IvyMechanic
{
    public class IvyBoardStateHandler
    {
        List<CellStack> ivyBushesCellStacks = new List<CellStack>();
        List<CellStack> ivyRootsCellStacks = new List<CellStack>();
        List<IvySackTile> ivySackTiles = new List<IvySackTile>();

        CellStack[] allCellStacks;
        CellStackBoard cellStackBoard;

        DestroyedObjectsData destroyedObjectsData;

        public IvyBoardStateHandler(GameBoard gameBoard, DestroyedObjectsData destroyedObjectsData)
        {
            this.allCellStacks = gameBoard.ArrbitrayCellStackArray();
            this.cellStackBoard = gameBoard.CellStackBoard();
            this.destroyedObjectsData = destroyedObjectsData;
        }


        public void TryUpdateIvyCellStacks()
        {
            if (destroyedObjectsData.tiles.Count > 0)
                UpdateIvyCellStacks();
        }

        public void UpdateIvyCellStacks()
        {
            ivyBushesCellStacks.Clear();
            ivyRootsCellStacks.Clear();
            ivySackTiles.Clear();
            foreach (var cellStack in allCellStacks)
            {
                if (HasTileOnTop<IvyBush>(cellStack))
                    ivyBushesCellStacks.Add(cellStack);
                if (HasCellOnTop<IvyRootCell>(cellStack))
                    ivyRootsCellStacks.Add(cellStack);
                if (HasTile<IvySackTile>(cellStack))
                    ivySackTiles.Add(FindTile<IvySackTile>(cellStack));
            }
        }


        public bool ShouldBeDisable()
        {
            return ivyRootsCellStacks.Count == 0 && ivySackTiles.Count == 0;
        }

        public bool CanGrow()
        {
            return NoIvyDestroyed() && NoIvyIsBeingDestroyed();
        }

        private bool NoIvyDestroyed()
        {
            foreach (var tile in destroyedObjectsData.tiles)
                if (tile is IvyBush)
                    return false;

            foreach (var cell in destroyedObjectsData.cells)
                if (cell is IvyRootCell)
                    return false;

            return true;
        }

        // NOTE: For now we assume if it is locked, it means it is going to be destoryed.
        private bool NoIvyIsBeingDestroyed()
        {
            foreach (var cellStack in ivyBushesCellStacks)
                if (IsFullyFree(cellStack) == false)
                    return false;

            foreach (var cellStack in ivyRootsCellStacks)
                if (IsFullyFree(cellStack) == false)
                    return false;

            foreach (var cellStack in ivyBushesCellStacks)
                if (IsAnySideLockedByRainbow(cellStack))
                    return false;

            foreach (var cellStack in ivyRootsCellStacks)
                if (IsAnySideLockedByRainbow(cellStack))
                    return false;

            return true;
        }

        private bool IsAnySideLockedByRainbow(CellStack cellStack)
        {
            var pos = cellStack.Position();
            var top = cellStackBoard.DirectionalElementOf(pos, Direction.Up);
            var bottom = cellStackBoard.DirectionalElementOf(pos, Direction.Down);
            var left = cellStackBoard.DirectionalElementOf(pos, Direction.Left);
            var right = cellStackBoard.DirectionalElementOf(pos, Direction.Right);

            return IsLockedByRainbow(top) || IsLockedByRainbow(bottom) || IsLockedByRainbow(left) || IsLockedByRainbow(right);
        }

        private bool IsLockedByRainbow(CellStack cellStack)
        {
            return
                cellStack != null
                && cellStack.HasTileStack()
                && cellStack.CurrentTileStack().GetComponent<LockState>().IsLockedBy<RainbowMechanic.RainbowActivateKeyType>();
        }

        public bool CanGrowRootIn(CellStack cellStack)
        {
            return HasCellOnTop<IvyRootCell>(cellStack) == false;
        }

        public void AddIvyBushesCellStack(CellStack cellStack)
        {
            ivyBushesCellStacks.Add(cellStack);
        }

        public void AddToIvyRootsCellStack(CellStack cellStack)
        {
            ivyRootsCellStacks.Add(cellStack);
        }


        public List<CellStack> IvyRootsCellStacks()
        {
            return ivyRootsCellStacks;
        }

        public List<CellStack> IvyBushesCellStacks()
        {
            return ivyBushesCellStacks;
        }
    }
}