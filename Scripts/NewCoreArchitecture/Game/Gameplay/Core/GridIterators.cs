using System;
using System.Collections;
using System.Collections.Generic;

// TODO : Refactor/Reorganize this.
namespace Match3.Game.Gameplay.Core
{
    public interface GridEnumratorIndexMover
    {
        void MoveNext(ref int x, ref int y, int width , int height);

        void Reset(ref int x, ref int y, int width, int height);
    }


    public struct GridEnumrator<T> : IEnumerator
    {
        Grid<T> grid;
        GridElement<T> gridElement;
        GridEnumratorIndexMover indexMover;
        int x;
        int y;
        int width;
        int height;

        public GridEnumrator(Grid<T> grid, GridEnumratorIndexMover indexMover)
        {
            x = 0;
            y = 0;
            this.indexMover = indexMover;
            this.grid = grid;

            width = grid.Width();
            height = grid.Height();

            indexMover.Reset(ref x, ref y, grid.Width(), grid.Height());
            gridElement = new GridElement<T>();
        }


        public GridElement<T> Current
        {
            get
            {
                try
                {
                    gridElement.x = x;
                    gridElement.y = y;
                    gridElement.value = grid[x, y];
                    return gridElement;
                }
                catch (IndexOutOfRangeException)
                {
                    throw new InvalidOperationException();
                }
            }
        }

        object IEnumerator.Current
        {
            get
            {
                return Current;
            }
        }

        public void Dispose()
        {
            grid = null;
        }

        public bool MoveNext()
        {

            indexMover.MoveNext(ref x, ref y, width, height);


            return grid.IsInRange(x, y);
        }

        public void Reset()
        {
            indexMover.Reset(ref x, ref y, grid.Width(), grid.Height());
        }
    }

    public struct GridElement<T>
    {
        public int x;
        public int y;
        public T value;
    }


    public abstract class GridIterator<T> : IEnumerable
    {
        public Grid<T> grid;

        protected GridElement<T> gridCell = new GridElement<T>();

        public GridIterator(Grid<T> grid)
        {
            this.grid = grid;
        }

        public abstract GridEnumrator<T> GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator) GetEnumerator();
        }

        public  void Iterate(Action<GridElement<T>> action)
        {
            foreach (var element in this)
                action(element);
        }
    }


    public class LeftToRightBottomUpGridIterator<T> : GridIterator<T>
    {
        class LeftToRightButtomUpIndexMover : GridEnumratorIndexMover
        {
            public void MoveNext(ref int x, ref int y, int width, int height)
            {
                x++;
                if (x == width)
                {
                    x = 0;
                    y--;
                }
            }

            public void Reset(ref int x, ref int y, int width, int height)
            {
                x = -1;
                y = height-1;
            }
        }

        GridEnumratorIndexMover indexMover = new LeftToRightButtomUpIndexMover();

        public LeftToRightBottomUpGridIterator(Grid<T> grid) : base(grid)
        {
        }

        public override GridEnumrator<T> GetEnumerator()
        {
            return new GridEnumrator<T>(grid, indexMover);
        }

    }

    public class LeftToRightTopDownGridIterator<T> : GridIterator<T>
    {
        class LeftToRightButtomUpIndexMover : GridEnumratorIndexMover
        {
            public void MoveNext(ref int x, ref int y, int width, int height)
            {
                x++;
                if (x == width)
                {
                    x = 0;
                    y++;
                }
            }

            public void Reset(ref int x, ref int y, int width, int height)
            {
                x = -1;
                y = 0;
            }
        }

        GridEnumratorIndexMover indexMover = new LeftToRightButtomUpIndexMover();

        public LeftToRightTopDownGridIterator(Grid<T> grid) : base(grid)
        {
        }

        public override GridEnumrator<T> GetEnumerator()
        {
            return new GridEnumrator<T>(grid, indexMover);
        }
    }

}