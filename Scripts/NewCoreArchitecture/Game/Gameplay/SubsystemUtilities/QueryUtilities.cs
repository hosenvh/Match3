
using System;
using System.Collections.Generic;
using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.HitManagement;

namespace Match3.Game.Gameplay
{
    public static class QueryUtilities 
    {
        public static bool IsFullyFree(CellStack cellStack)
        {
            return IsFree(cellStack) && (cellStack.HasTileStack() == false || IsFree(cellStack.CurrentTileStack()));
        }

        public static bool IsFree(CellStack cellStack)
        {
            return cellStack.componentCache.lockState.IsFree();
        }

        public static bool IsLockedBy<T>(CellStack cellStack) where T : KeyType
        {
            return cellStack.componentCache.lockState.IsLockedBy<T>();
        }

        public static bool IsLockedBy<T>(TileStack tileStack) where T : KeyType
        {
            return tileStack.componentCache.lockState.IsLockedBy<T>();
        }

        public static bool IsFree(TileStack tileStack)
        {
            return tileStack.componentCache.lockState.IsFree();
        }

        public static bool IsFullyFree(TileStack tileStack)
        {
            return IsFree(tileStack) && IsFree(tileStack.Parent());
        }

        public static List<T> ExtractTilesOnTop<T>(CellStack[] cellStacks) where T : Tile
        {
            List<T> tiles = new List<T>();

            for (int i = 0; i < cellStacks.Length; ++i)
            {
                var cellStack = cellStacks[i];

                if (HasAnyTile(cellStack) && TopTile(cellStack) is T target)
                    tiles.Add(target);
            }

            return tiles;
        }

        public static List<T> ExtractCellsOnTop<T>(CellStack[] cellStacks, Predicate<T> predicate) where T : Cell
        {
            List<T> cells = new List<T>();

            for (int i = 0; i < cellStacks.Length; ++i)
            {
                var cellStack = cellStacks[i];

                if (cellStack.Top() is T target && predicate.Invoke(target))
                    cells.Add(target);
            }

            return cells;
        }




        public static bool HasTileOnTop<T>(TileStack tileStack) where T : Tile
        {
            return tileStack != null && tileStack.IsDepleted() == false && tileStack.Top() is T;
        }

        public static bool HasTileOnTop<T>(CellStack cellStack) where T : Tile
        {
            return HasTileOnTop<T>(cellStack.CurrentTileStack());
        }

        public static bool HasTileOnTop(CellStack cellStack, Type tileType)
        {
            return HasAnyTile(cellStack) && tileType.IsAssignableFrom(TopTile(cellStack).GetType());
        }

        public static bool HasCellOnTop<T>(CellStack cellStack) where T : Cell
        {
            return cellStack.Top() is T;
        }


        public static bool HasTile(CellStack cellStack, Type tileType)
        {
            if (HasAnyTile(cellStack) == false)
                return false;

            foreach (var tile in cellStack.CurrentTileStack().Stack())
                if (tileType.IsAssignableFrom(tile.GetType()))
                    return true;
            return false;
        }

        public static bool HasTile<T>(CellStack cellStack) where T : Tile
        {
            return HasTile(cellStack, typeof(T));
        }

        public static T FindTile<T>(CellStack cellStack) where T : Tile
        {
            return FindTile<T>(cellStack.CurrentTileStack());
        }

        public static T FindTile<T>(TileStack tileStack) where T : Tile
        {
            foreach (var tile in tileStack.Stack())
                if (tile is T)
                    return (T)tile;

            return default(T);
        }

        public static T FindCell<T>(CellStack cellStack) where T : Cell
        {
            foreach (var cell in cellStack.Stack())
                if (cell is T)
                    return (T)cell;

            return default(T);
        }


        public static Tile TileBelowOf(Tile tile)
        {
            bool passedTargetTile = false;

            foreach(var t in tile.Parent().Stack())
            {
                if (t == tile)
                    passedTargetTile = true;

                else if (passedTargetTile)
                    return t;
            }

            return null;
        }

        public static bool HasAnyTile(CellStack target)
        {
            return target.HasTileStack() && target.CurrentTileStack().IsDepleted() == false;
        }


        public static Tile TopTile(CellStack cellStack)
        {
            return cellStack.CurrentTileStack().Top();
        }

        public static int DistanceOf(CellStack origin, CellStack destination)
        {
            return
                Math.Abs(origin.Position().x - destination.Position().x)
                + Math.Abs(origin.Position().y - destination.Position().y);
        }


        public static bool IsGoingToBeHit(CellStack cellStack, CellStackBoard cellStackBoard)
        {
            if (IsLockedBy<HitApplyingKeyType>(cellStack))
                return true;

            if (cellStack.HasTileStack() && IsLockedBy<HitApplyingKeyType>(cellStack.CurrentTileStack()))
                return true;

            var pos = cellStack.Position();
            var top = cellStackBoard.DirectionalElementOf(pos, Direction.Up);
            var bottom = cellStackBoard.DirectionalElementOf(pos, Direction.Down);
            var left = cellStackBoard.DirectionalElementOf(pos, Direction.Left);
            var right = cellStackBoard.DirectionalElementOf(pos, Direction.Right);

            return IsLockedByRainbow(top) || IsLockedByRainbow(bottom) || IsLockedByRainbow(left) || IsLockedByRainbow(right);
                
        }

        public static T FindFirstTileInBoard<T>(GameBoard gameBoard) where T : Tile
        {
            foreach(var cellStack in gameBoard.ArrbitrayCellStackArray())
            {
                if (cellStack.HasTileStack() == false)
                    continue;

                var target = FindTile<T>(cellStack);
                if (target != null)
                    return target;
            }

            return null;
        }

        static bool IsLockedByRainbow(CellStack cellStack)
        {
            return
                cellStack != null
                && cellStack.HasTileStack()
                && cellStack.CurrentTileStack().GetComponent<LockState>().IsLockedBy<RainbowMechanic.RainbowActivateKeyType>();
        }


    }

}