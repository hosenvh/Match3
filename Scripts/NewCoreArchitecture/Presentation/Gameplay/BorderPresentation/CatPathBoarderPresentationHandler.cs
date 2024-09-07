using Match3.Game.Gameplay.Cells;
using Match3.Game.Gameplay.Core;

namespace Match3.Presentation.Gameplay.BorderPresentation
{
    public class CatPathBoarderPresentationHandler : BorderPresentationHandler
    {
        protected override void SetupBoarderInfo(int x, int y, CellStack value)
        {
            if (value.Top() is CatPathCell)
            {
                borderInfos[x, y].FillSE();
                borderInfos[x, y + 1].FillNE();
                borderInfos[x + 1, y].FillSW();
                borderInfos[x + 1, y + 1].FillNW();
            }
        }
    }
}