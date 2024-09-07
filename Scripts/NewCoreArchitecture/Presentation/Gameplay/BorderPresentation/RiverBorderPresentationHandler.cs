using Match3.Game.Gameplay.Cells;
using Match3.Game.Gameplay.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Match3.Presentation.Gameplay.BorderPresentation
{

    public class RiverBorderPresentationHandler : BorderPresentationHandler
    {
        protected override void SetupBoarderInfo(int x, int y, CellStack value)
        {
            if (value.Top() is RiverCell)
            {
                borderInfos[x, y].FillSE();
                borderInfos[x, y + 1].FillNE();
                borderInfos[x + 1, y].FillSW();
                borderInfos[x + 1, y + 1].FillNW();
            }
        }



    }
}