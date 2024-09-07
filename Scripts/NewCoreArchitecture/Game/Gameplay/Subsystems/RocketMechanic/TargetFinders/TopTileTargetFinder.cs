using Match3.Game.Gameplay.Core;
using System.Collections.Generic;


namespace Match3.Game.Gameplay.SubSystems.RocketMechanic
{
    public class TopTileTargetFinder<T> : TargetFinder where T : Tile
    {
        public void Find(GameBoard gameBoard, ref List<CellStack> targets)
        {
            foreach (var cellStack in gameBoard.ArrbitrayCellStackArray())
                if (QueryUtilities.HasTileOnTop<T>(cellStack.CurrentTileStack()) && QueryUtilities.IsFullyFree(cellStack))
                    targets.Add(cellStack);
        }
    }
}