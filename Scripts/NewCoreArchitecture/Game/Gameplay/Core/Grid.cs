using System;
using System.Collections.Generic;
using UnityEngine;

namespace Match3.Game.Gameplay.Core
{
    public enum Direction { None, Up, Down, Left, Right }
    public enum GridPlacement { None, Up, Down, Left, Right }

    public struct DirectionComparer : IEqualityComparer<Direction>
    {
        public bool Equals(Direction x, Direction y)
        {
            return x == y;
        }

        public int GetHashCode(Direction obj)
        {
            // you need to do some thinking here,
            return (int)obj;
        }
    }

    public class Grid<T>
    {
        public T[][] internalGrid;

        int width;
        int height;

        public Grid(int width, int height)
        {
            this.width = width;
            this.height = height;

            internalGrid = new T[height][];
            for (int j = 0; j < height; ++j)
                internalGrid[j] = new T[width]; 
        }


        public T this[int x, int y]
        {
            get
            {
                if (IsInRange(x, y) == false)
                    return default(T);
                return internalGrid[y][x];
            }
            set
            {
                internalGrid[y][x] = value;
            }
        }

        public T this[Vector2Int pos]
        {
            get
            {
                return this[pos.x, pos.y];
            }
            set
            {
                this[pos.x, pos.y] = value;
            }
        }

        public bool IsInRange(int x, int y)
        {
            return 0 <= x && x < width && 0 <= y && y < height;
        }

        [Obsolete("Use Gird Iterators")]
        public void ForEach(Action<int, int, T> action)
        {
            for (int j = 0; j < height; ++j)
                for (int i = 0; i < width; ++i)
                    action(i, j, this[i, j]);
        }

        public int PositionToLinearIndex(int x, int y)
        {
            return y * width + x;
        }

        public int PositionToLinearIndex(Vector2Int position)
        {
            return PositionToLinearIndex(position.x, position.y);
        }

        public Vector2Int LinearIndexToPosition(int linearIndex)
        {
            return new Vector2Int(linearIndex % width, linearIndex / width);
        }

        public T BottomOf(int x, int y)
        {
            return DirectionalElementOf(x, y, Direction.Down);
        }

        public T BottomOf(Vector2Int position)
        {
            return BottomOf(position.x, position.y);
        }

        public T TopOf(int x, int y)
        {
            return DirectionalElementOf(x, y, Direction.Up);
        }

        // TODO: Find a better name.
        public T DirectionalElementOf(int x, int y, Direction dir)
        {
            switch (dir)
            {
                case Direction.Down:
                    return this[x, y + 1];
                case Direction.Up:
                    return this[x, y - 1];
                case Direction.Left:
                    return this[x - 1, y];
                case Direction.Right:
                    return this[x + 1, y];
            }

            return default(T);
        }

        public Direction GetOppositeDirectionOf(Direction dir)
        {
            switch (dir)
            {
                case Direction.Down:
                    return Direction.Up;
                case Direction.Up:
                    return Direction.Down;
                case Direction.Left:
                    return Direction.Right;
                case Direction.Right:
                    return Direction.Left;
            }
            return Direction.None;
        }


        public T DirectionalElementOf(Vector2Int pos, Direction dir)
        {
            return DirectionalElementOf(pos.x, pos.y, dir);
        }

        // TODO: Test this.
        public bool AreAdjacent(Vector2Int pos1, Vector2Int pos2)
        {
            return Vector2Int.Distance(pos1, pos2) <= 1;
        }

        public Direction RelativeDirectionOf(Vector2Int from, Vector2Int to)
        {
            if (AreAdjacent(from, to))
            {
                var xDist = to.x - from.x;
                var yDist = to.y - from.y;

                if (xDist >= 1)
                    return Direction.Right;
                else if (xDist <= -1)
                    return Direction.Left;
                else if (yDist >= 1)
                    return Direction.Down;
                else if (yDist <= -1)
                    return Direction.Up;

            }
            
            throw new Exception();
        }

        public int Width()
        {
            return width;
        }

        public int Height()
        {
            return height;
        }

        public Vector2Int Size()
        {
            return new Vector2Int(width, height);
        }

    }
}