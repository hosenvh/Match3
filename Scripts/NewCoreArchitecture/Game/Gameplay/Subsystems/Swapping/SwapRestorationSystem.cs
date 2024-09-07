

using System;
using Match3.Foundation.Base.EventManagement;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.Input;
using Match3.Game.Gameplay.RainbowMechanic;
using Match3.Game.Gameplay.Tiles;
using static Match3.Game.Gameplay.ActionUtilites;
using static Match3.Game.Gameplay.QueryUtilities;

namespace Match3.Game.Gameplay.Swapping
{
    public struct SwapSuccessfulEvent : GameEvent
    { }

    public struct SwapFailedEvent : GameEvent
    { }

    [AfterAttribute(typeof(SwapExecutionSystem))]
    [AfterAttribute(typeof(Matching.MatchDetectionSystem))]
    public class SwapRestorationSystem : GameplaySystem
    {

        TileStackSwapHelper swapHelper;

        EventManager eventManager;

        public SwapRestorationSystem(GameplayController gameplayController) : base(gameplayController)
        {
            swapHelper = new TileStackSwapHelper(gameplayController);
            eventManager = ServiceLocator.Find<EventManager>();
        }

        public override void Update(float dt)
        {
            var executedSwapsData = GetFrameData<ExecutedSwapsData>();

            foreach (var swapData in executedSwapsData.data)
            {
                if (ShouldRestoreSwap(swapData.originTarget, swapData.destinationTarget))
                {
                    Lock(swapData.originTarget, swapData.destinationTarget);
                    swapHelper.Process(swapData.originTarget, swapData.destinationTarget, Unlock);
                    eventManager.Propagate(new SwapFailedEvent(), this);
                }
                else
                {
                    GetFrameData<SuccessfulSwapsData>().data.Add(swapData);
                    eventManager.Propagate(new SwapSuccessfulEvent(), this);
                }

            }

        }

        private void Unlock(CellStack cellStack1, CellStack cellStack2)
        {
            FullyUnlock(cellStack1);
            FullyUnlock(cellStack2);
        }

        private void Lock(CellStack cellStack1, CellStack cellStack2)
        {
            FullyLock<SwapRestorationKeyType>(cellStack1);
            FullyLock<SwapRestorationKeyType>(cellStack2);
        }

        private SwapRequestData ConvertSwapData(SwapRequestData swapData)
        {
            return new SwapRequestData(swapData.target, Inverse(swapData.direction));
        }

        private Direction Inverse(Direction direction)
        {
            switch(direction)
            {
                case Direction.Down:
                    return Direction.Up;
                case Direction.Up:
                    return Direction.Down;
                case Direction.Left:
                    return Direction.Right;
                case Direction.Right:
                    return Direction.Left;
            }

            throw new Exception();
        }
        private bool ShouldRestoreSwap(CellStack target1, CellStack target2)
        {
            if (MatchMadeFor(target1) || HasTileOnTop<ExplosiveTile>(target1) || MatchMadeFor(target2) || HasTileOnTop<ExplosiveTile>(target2))
                return false;

            if (HasTileOnTop<Rainbow>(target1) && HasAnyTile(target2) && target2.CurrentTileStack().Top().GetComponent<TileRainbowProperties>().DoesMatchWithRainbow())
                return false;

            if (HasTileOnTop<Rainbow>(target2) && HasAnyTile(target1) && target1.CurrentTileStack().Top().GetComponent<TileRainbowProperties>().DoesMatchWithRainbow())
                return false;

            return true;
        }

        bool MatchMadeFor(CellStack target)
        {
            if (target.HasTileStack() == false)
                return false;

            foreach (var match in GetFrameData<CreatedMatchesData>().data)
                if (match.tileStacks.Contains(target.CurrentTileStack()))
                    return true;
            return false;
        }
 
    }
}