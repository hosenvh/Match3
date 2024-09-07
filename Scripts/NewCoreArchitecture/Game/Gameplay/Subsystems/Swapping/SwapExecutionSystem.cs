using System;
using Match3.Game.Gameplay.Core;
using static Match3.Game.Gameplay.ActionUtilites;
using static Match3.Game.Gameplay.QueryUtilities;

namespace Match3.Game.Gameplay.Swapping
{
    // NOTE: It is important to consider that the destination of a swap may not have a TileStack;
    [AfterAttribute(typeof(Input.InputSystem))]
    public class SwapExecutionSystem : GameplaySystem
    {

        TileStackSwapHelper swapHelper;

        ExecutedSwapsData executedSwapsData;
        RequestedSwapsData requestedSwapsData;

        public SwapExecutionSystem(GameplayController gameplayController) : base(gameplayController)
        {
            swapHelper = new TileStackSwapHelper(gameplayController);

            executedSwapsData = GetFrameData<ExecutedSwapsData>();
            requestedSwapsData = GetFrameData<RequestedSwapsData>();
        }


        public override void Update(float dt)
        {
            foreach (var swapData in requestedSwapsData.data)
            {
                if (IsBlockedByWall(swapData))
                    continue;

                var originTileStack = swapData.target;
                var destinationCellStack = DetermineDestinationCellStack(swapData);

                if (IsTileStackValidForSwapping(originTileStack) && IsCellStackValidForSwapping(destinationCellStack))
                {
                    FullyLock<SwapExecutionKeyType>(originTileStack.Parent());
                    FullyLock<SwapExecutionKeyType>(destinationCellStack);

                    swapHelper.Process(originTileStack.Parent(), destinationCellStack, OnSwapCompleted);
                }
            }
            requestedSwapsData.data.Clear();
        }

        private bool IsBlockedByWall(SwapRequestData swapData)
        {
            return gameplayController.boardBlockageController.IsBlocked(swapData.target.Parent(), swapData.direction);
        }

        private bool IsTileStackValidForSwapping(TileStack target)
        {
            return target != null
                && IsFullyFree(target.Parent())
                && IsNotInSwapExecutionData(target)
                && (target.IsDepleted()  || target.Top().GetComponent<TileUserInteractionProperties>().isSwappable);
        }

        private bool IsCellStackValidForSwapping(CellStack target)
        {
            if (
                target == null 
                || target.Top().CanContainTile() == false 
                || IsFree(target) == false
                )
                return false;

            return target.HasTileStack() == false 
                || target.CurrentTileStack().IsDepleted() 
                || IsTileStackValidForSwapping(target.CurrentTileStack());
        }

        private bool IsNotInSwapExecutionData(TileStack tileStack)
        {
            foreach (var data in executedSwapsData.data)
                if (data.originTarget.CurrentTileStack() == tileStack || data.destinationTarget.CurrentTileStack() == tileStack)
                    return false;

            return true;
        }

        public CellStack DetermineDestinationCellStack(SwapRequestData swapData)
        {
            var pos = swapData.target.Parent().Position();
            var cellStack = gameplayController.GameBoard().
                DirectionalCellStackOf(
                    pos.x,
                    pos.y,
                    swapData.direction);

            return cellStack;
        }


        private  void OnSwapCompleted(CellStack originCellStack, CellStack destinationCellStack)
        {
            FullyUnlock(originCellStack);
            FullyUnlock(destinationCellStack);
            GetFrameData<ExecutedSwapsData>().data.Add(new SwapExecutionData(originCellStack, destinationCellStack));
        }
    }
}