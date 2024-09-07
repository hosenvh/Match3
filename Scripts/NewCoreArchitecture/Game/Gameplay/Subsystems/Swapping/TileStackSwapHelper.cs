using Match3.Game.Gameplay.Core;
using System;

namespace Match3.Game.Gameplay.Swapping
{

    public interface SwappingPresentationHandler : PresentationHandler
    {
        void HandleSwap(CellStack origin, CellStack destination, Action onComplete);
    }

    // TODO: Refactor this shit.
    public class TileStackSwapHelper
    {

      
        GameplayController gameplayController;

        SwappingPresentationHandler presentationHandler;

        public TileStackSwapHelper(GameplayController gameplayController)
        {
            this.gameplayController = gameplayController;

            presentationHandler = gameplayController.GetPresentationHandler<SwappingPresentationHandler>();
        }


        public void Process(CellStack origin, CellStack destination, Action<CellStack, CellStack> onCompleted)
        {
            StartSwappingTiles(origin, destination, onCompleted);
        }


        private void StartSwappingTiles(CellStack cellStack1, CellStack cellStack2, Action<CellStack, CellStack> onCompleted)
        {
            presentationHandler.HandleSwap(cellStack1, cellStack2, () => DoSwap(cellStack1, cellStack2, onCompleted));
        }

        private void DoSwap(CellStack cellStack1, CellStack cellStack2, Action<CellStack, CellStack> onCompleted)
        {
            var tilestack1 = cellStack1.CurrentTileStack();
            var tilestack2 = cellStack2.CurrentTileStack();

            if (tilestack1 != null)
                cellStack2.SetCurrnetTileStack(tilestack1);
            else
                cellStack2.DetachTileStack();

            if (tilestack2 != null)
                cellStack1.SetCurrnetTileStack(tilestack2);
            else
                cellStack1.DetachTileStack();

            if(cellStack1.HasTileStack())
                cellStack1.CurrentTileStack().SetPosition(cellStack1.Position());
            if(cellStack2.HasTileStack())
                cellStack2.CurrentTileStack().SetPosition(cellStack2.Position());

            onCompleted(cellStack1, cellStack2);
        }

    }
}