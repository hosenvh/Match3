
using Match3.Game.Gameplay.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Match3.Game.Gameplay.HitManagement
{

    // TODO: Rename this.
    public class SideTileFinder
    {
        GameBoard gameBoard;


        public SideTileFinder(GameBoard gameBoard)
        {
            this.gameBoard = gameBoard;
        }

        public void Find(TileStack tileStack, ref HashSet<CellStack> output)
        {
            var positon = tileStack.Parent().Position();
            Find(positon, ref output);
        }


        public void Find(CellStack cellStack, ref HashSet<CellStack> output)
        {
            Find(cellStack.Position(), ref output);
        }

        public void Find(Vector2Int position, ref HashSet<CellStack> output)
        {
            TryAdd(gameBoard.DirectionalCellStackOf(position, Direction.Down), ref output);
            TryAdd(gameBoard.DirectionalCellStackOf(position, Direction.Up), ref output);
            TryAdd(gameBoard.DirectionalCellStackOf(position, Direction.Left), ref output);
            TryAdd(gameBoard.DirectionalCellStackOf(position, Direction.Right), ref output);
        }

        private void TryAdd(CellStack cellStack, ref HashSet<CellStack> output)
        {
            if (cellStack != null)
            {
                output.Add(cellStack);
            }
        }

    }
}