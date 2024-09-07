using Match3.Game.Gameplay.Core;

namespace Match3.Game.Gameplay
{
    public class BoardStabilityCalculator
    {

        CellStack[] cellStacks;

        public BoardStabilityCalculator(GameBoard gameBoard)
        {
            cellStacks = gameBoard.leftToRightButtomUpCellStackArray;
        }

        public bool CalculateIsStable()
        {
            foreach (var cellStack in cellStacks)
            {
                if (IsStable(cellStack) == false)
                    return false;  
            }

            return true;
        }
        private bool IsStable(CellStack value)
        {
            var tileStack = value.CurrentTileStack();
            return tileStack == null || tileStack.GetComponent<LockState>(0).IsFree();
        }
    }
}