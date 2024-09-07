using System.Collections.Generic;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.Factories;
using Match3.Game.Gameplay.SubSystems.LevelEnding;
using Match3.Game.Gameplay.Tiles;

namespace Match3.Game.Gameplay.SubSystems.WinSequence
{
    public class PlacingState : State
    {
        int remainingMoves;
        bool isDepleted;
        WinSequencePresentationHandler presentationHandler;


        protected override void InternalSetup()
        {
            presentationHandler = gpc.GetPresentationHandler<WinSequencePresentationHandler>();
            remainingMoves = gpc.GetSystem<LevelStoppingSystem>().FindStoppingCondition<MovementStopCondition>().RemainingMovements();
            isDepleted = remainingMoves <= 0;
        }


        public override void Update(float dt)
        {
            if (IsDepleted())
                return;


            var chooser = new RandomCellStackChooser(gpc.GameBoard().CellStackBoard());
            var chosenCellStacks = chooser.Choose(remainingMoves, IsValidForPlacing);

            var tiles = new List<Tile>();


            ActionUtilites.LockTileStacksBy<WinSequenceKeyType>(chosenCellStacks);

            foreach (var cellStack in chosenCellStacks)
                tiles.Add(system.CreateAnExplosive());


            presentationHandler.PlaceExplosives(
                tiles,
                chosenCellStacks,
                (i) => ApplyCreation(tiles[i], chosenCellStacks[i]),
                () => OnPlacementSkipOrFinished(chosenCellStacks));

            isDepleted = true;

        }

        void OnPlacementSkipOrFinished(List<CellStack> chosenCellStacks)
        {
            ActionUtilites.UnlockTileStacksWith<WinSequenceKeyType>(chosenCellStacks);
            system.OnPlacementFinished();
        }

        void ApplyCreation(Tile tile, CellStack cellStack)
        {
            gpc.creationUtility.ReplaceTileInBoard(tile, cellStack);
            remainingMoves--;
        }

        bool IsValidForPlacing(CellStack cellStack)
        {
            return cellStack.Top().CanContainTile() 
                && cellStack.HasTileStack() 
                && cellStack.CurrentTileStack().IsDepleted() == false
                && cellStack.CurrentTileStack().Top() is ColoredBead;
        }

        public int RemainingMoves()
        {
            return remainingMoves;
        }

        public bool IsDepleted()
        {
            return isDepleted;
        }
    }
}