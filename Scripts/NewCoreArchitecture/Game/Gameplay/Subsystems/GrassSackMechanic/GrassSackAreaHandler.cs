using Match3.Game.Gameplay.Core;
using System;
using UnityEngine;

namespace Match3.Game.Gameplay.SubSystems.GrassSackMechanic
{
    public class GrassSackAreaHandler
    {
        CellStackBoard cellStackBoard;

        public GrassSackAreaHandler(CellStackBoard cellStackBoard)
        {
            this.cellStackBoard = cellStackBoard;
        }

        public void ForEachOuterCellStackDo(Vector2Int origin, Action<CellStack, int> action)
        {
            int left = origin.x - 2;
            int right = origin.x + 3;
            int top = origin.y - 2;
            int bottom = origin.y + 3;

            for (int i = left + 1; i <= right - 1; ++i)
            {
                if (IsValidForCreation(i, top))
                    action.Invoke(cellStackBoard[i, top], DistanceFromSack(origin, i, top));
                if (IsValidForCreation(i, bottom))
                    action.Invoke(cellStackBoard[i, bottom], DistanceFromSack(origin, i, bottom));
            }

            for (int j = top + 1; j <= bottom - 1; ++j)
            {
                if (IsValidForCreation(left, j))
                    action.Invoke(cellStackBoard[left, j], DistanceFromSack(origin, left, j));
                if (IsValidForCreation(right, j))
                    action.Invoke(cellStackBoard[right, j], DistanceFromSack(origin, right, j));
            }
        }

        public void ForEachInnerCellStackDo(Vector2Int sackOrigin, Action<CellStack, int> action)
        {
            int left = sackOrigin.x - 1;
            int right = sackOrigin.x + 2;
            int top = sackOrigin.y - 1;
            int bottom = sackOrigin.y + 2;

            for (int i = left; i <= right; ++i)
                for (int j = top; j <= bottom; ++j)
                    if (IsValidForCreation(i, j))
                        action.Invoke(cellStackBoard[i, j], DistanceFromSack(sackOrigin, i, j));
        }

        private bool IsValidForCreation(int x , int y)
        {
            return cellStackBoard.IsInRange(x, y) && cellStackBoard[x, y].Top().GetComponent<GrassSackMechanicCellProperties>().canCreateItemOnCell;
        }

        private int DistanceFromSack(Vector2Int topLeftCorner, int x, int y)
        {
            int xDistance = Math.Abs(topLeftCorner.x - x);
            int yDistance = Math.Abs(topLeftCorner.y - y);

            if (x > topLeftCorner.x)
                xDistance -= 1;

            if (y > topLeftCorner.y)
                yDistance -= 1;

            return Math.Max(xDistance, yDistance);
        }
    }
}