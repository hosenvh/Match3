using Match3.Game.Gameplay;
using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.Physics;
using Match3.Game.Gameplay.SubSystems.ButterflyMechanic;
using Match3.Game.Gameplay.Tiles;

namespace Match3.Presentation.Gameplay.BorderPresentation
{
    public class SandBorderPresentationHandler : DynamicBorderPresentationHandler
    {
        protected override bool ShouldConsiderBordersOf(CellStack cellStack)
        {
            if (cellStack.HasTileStack() == false)
                return false;

            // NOTE: Just checking the general lock state will cause some visual issues.
            return
                cellStack.CurrentTileStack().IsDepleted() == false
                && cellStack.CurrentTileStack().Top() is Sand
                && cellStack.CurrentTileStack().GetComponent<TileStackPhysicalState>().PhysicsState() != PhysicsState.Falling
                && cellStack.CurrentTileStack().GetComponent<LockState>().IsLockedBy<ButterflyMovementKeyType>() == false;


        }
    }
}