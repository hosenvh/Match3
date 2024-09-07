
using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.SubSystems.HoneyMechanic;
using Match3.Game.Gameplay.Tiles;
using UnityEngine;
using static Match3.Game.Gameplay.QueryUtilities;

namespace Match3.Presentation.Gameplay.BorderPresentation
{
    public class HoneyBorderPresentationHandler : DynamicBorderPresentationHandler
    {
        protected override bool ShouldConsiderBordersOf(CellStack cellStack)
        {
            if (cellStack.HasTileStack() == false)
                return false;

            return HasTileOnTop<Honey>(cellStack) || HasTile<Honeycomb>(cellStack);

        }
    }
}