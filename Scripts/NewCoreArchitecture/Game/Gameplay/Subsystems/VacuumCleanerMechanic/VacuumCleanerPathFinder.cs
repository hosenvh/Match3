
using Match3.Game.Gameplay.Cells;
using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.Tiles;
using System.Collections.Generic;

namespace Match3.Game.Gameplay.SubSystems.VacuumCleanerMechanic
{
    public class VacuumCleanerPathFinder
    {
        GameBoard gameBoard;


        public VacuumCleanerPathFinder(GameBoard gameBoard)
        {
            this.gameBoard = gameBoard;

        }

        public List<CellStack> FindFor(VacuumCleaner vacuumCleaner)
        {
            List<CellStack> path = new List<CellStack>();

            var origin = vacuumCleaner.Parent().Parent();
            var currentCell = gameBoard.DirectionalCellStackOf(origin.Position().x, origin.Position().y, vacuumCleaner.direction);

            while(currentCell != null)
            {
                path.Add(currentCell);
                currentCell = gameBoard.DirectionalCellStackOf(currentCell.Position().x, currentCell.Position().y, vacuumCleaner.direction);
            }

            path.Reverse();

            for(int i = 0; i < path.Count; ++i)
                if(path[i].Top() is EmptyCell == false)
                {
                    path.RemoveRange(0, i);
                    break;
                }

            path.Reverse();
            return path;
        }
    }
}