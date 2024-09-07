
using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.SubSystems.HoneyMechanic;
using Match3.Game.Gameplay.Tiles;
using static Match3.Game.Gameplay.QueryUtilities;

namespace Match3.Presentation.Gameplay.BorderPresentation
{
    public class HardenedHoneyBorderPresentationHandler : DynamicBorderPresentationHandler
    {
        protected override bool ShouldConsiderBordersOf(CellStack cellStack)
        {
            if (cellStack.HasTileStack() == false)
                return false;

            return HasTileOnTop<HardenedHoney>(cellStack);

        }
    }
}