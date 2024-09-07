using System.Collections.Generic;
using Match3.Game.Gameplay.Core;

namespace Match3.Game.Gameplay
{
    public static class ActionUtilites
    {
        public static void LockTileStacksBy<T>(List<CellStack> cellStacks) where T :KeyType
        {
            foreach (var cellStack in cellStacks)
                if(cellStack.HasTileStack())
                    cellStack.CurrentTileStack().componentCache.lockState.LockBy<T>();
        }

        public static void UnlockTileStacksWith<T>(ICollection<CellStack> cellStacks) where T : KeyType
        {
            foreach (var cellStack in cellStacks)
                if (cellStack.HasTileStack())
                    UnlockWithKey<T>(cellStack.CurrentTileStack());
        }

        public static void ReleaseTileStacksIn(List<CellStack> cellStacks)
        {
            foreach (var cellStack in cellStacks)
                Unlock(cellStack.CurrentTileStack());
        }

        public static void Unlock(TileStack tileStack)
        {
            tileStack.componentCache.lockState.Release();
        }

        public static void Unlock(CellStack cellStack)
        {
            cellStack.componentCache.lockState.Release();
        }


        public static void TryUnlock(TileStack tileStack)
        {
            if (tileStack != null)
                Unlock(tileStack);
        }

        public static void UnlockWithKey<T>(TileStack tileStack) where T : KeyType
        {
            var lockState = tileStack.componentCache.lockState;
            if (lockState.IsLockedBy<T>())
                lockState.Release();
        }


        public static void Lock<T>(CellStack cellStack) where T : KeyType
        {
            cellStack.componentCache.lockState.LockBy<T>();
        }

        public static void Lock<T>(TileStack tileStack) where T : KeyType
        {
            tileStack.componentCache.lockState.LockBy<T>();
        }

        public static void TryLock<T>(TileStack tileStack) where T : KeyType
        {
            if (tileStack != null)
                Lock<T>(tileStack);
        }

        public static void FullyLock<T>(CellStack cellStack) where T : KeyType
        {
            Lock<T>(cellStack);
            TryLock<T>(cellStack.CurrentTileStack());
        }

        public static void FullyUnlock(CellStack cellStack)
        {
            Unlock(cellStack);
            TryUnlock(cellStack.CurrentTileStack());
        }

        public static void FullyDestroy(Tile tile)
        {
            var tileStack = tile.Parent();
            var cellStack = tileStack.Parent();
            FullyUnlock(cellStack);

            // WARNING: It is assumed that the tile is the top tile on the stack.
            tileStack.Pop();

            tile.ForceDestroy();
        }

        public static void SwapTileStacksOf(CellStack cellStack1, CellStack cellStack2)
        {
            var tilestack1 = cellStack1.CurrentTileStack();
            var tilestack2 = cellStack2.CurrentTileStack();

            if (tilestack1 != null)
                cellStack2.SetCurrnetTileStack(tilestack1);
            else
                cellStack2.DetachTileStack();

            if (tilestack2 != null)
                cellStack1.SetCurrnetTileStack(tilestack2);
            else
                cellStack1.DetachTileStack();

            if (cellStack1.HasTileStack())
                cellStack1.CurrentTileStack().SetPosition(cellStack1.Position());
            if (cellStack2.HasTileStack())
                cellStack2.CurrentTileStack().SetPosition(cellStack2.Position());

        }
    }

}