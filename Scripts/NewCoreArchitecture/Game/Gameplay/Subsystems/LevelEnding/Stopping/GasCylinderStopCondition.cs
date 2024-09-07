using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.Tiles;
using static Match3.Game.Gameplay.QueryUtilities;

namespace Match3.Game.Gameplay.SubSystems.LevelEnding
{
    public class GasCylinderStopCondition : StopConditinon
    {
        GameBoard gameBoard;

        public GasCylinderStopCondition(GameBoard gameBoard)
        {
            this.gameBoard = gameBoard;
        }

        public bool IsSatisfied()
        {
            CellStack[] cellStacks = gameBoard.ArrbitrayCellStackArray();
            var length = cellStacks.Length;
            for (int i = 0; i < length; ++i)
            {
                CellStack cellStack = cellStacks[i];
                if (IsFullyFree(cellStack) && cellStack.HasTileStack())
                {
                    var gasCylinder = FindTile<GasCylinder>(cellStack);
                    if(gasCylinder != null && gasCylinder.CurrentCountdown() <= 0)
                        return true;
                }

            }

            return false;

        }

        public void Update(float dt, LevelStoppingSystem system)
        {
        }
    }
}