using Match3.Game.Gameplay.Cells;
using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.SubSystems.IvyMechanic;
using Match3.Game.Gameplay.Tiles;
using Match3.Presentation.Gameplay.Cells;
using Match3.Presentation.Gameplay.Tiles;
using UnityEngine;
using static Match3.Game.Gameplay.QueryUtilities;

namespace Match3.Presentation.Gameplay.LogicPresentationHandlers
{
    public class IvyExpansionPresentationHandlerImp : MonoBehaviour, IvyExpansionPresentationHandler
    {
        public void ShowBushGrowIn(CellStack cellStack)
        {
            TopTile(cellStack).As<IvyBush>().GetComponent<IvyBushTilePresenter>().PlayGrowthEffect();
        }

        public void ShowRootGrowIn(CellStack cellStack)
        {
            cellStack.Top().As<IvyRootCell>().GetComponent<IvyRootCellPresenter>().PlayGrowthEffect();
        }
    }
}