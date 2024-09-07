using System;
using System.Collections.Generic;
using KitchenParadise.Utiltiy.Base;
using Match3.Game.Gameplay.Tiles;
using Match3.Game.Gameplay.Cells;
using Match3.Game.Gameplay.Core;

namespace Match3.Game.Gameplay.SubSystems.DuckMechanic
{
    public struct DuckMovementKeyType : KeyType
    { }

    public struct MovementData
    {
        public readonly Duck duck;
        public readonly RiverCell target;

        public MovementData (Duck duck, RiverCell target)
        {
            this.duck = duck;
            this.target = target;
        }
    }

    public interface DuckMovementPresentationHandler : PresentationHandler
    {
        void Move (List<MovementData> movementData, Action<MovementData> onCompleted);
    }

    [After(typeof(DestructionManagement.DestructionSystem))]
    [After(typeof(General.UserMovementManagementSystem))]
    public class DuckMovementSystem : DuckSystem
    {
        private const int TURNS_TO_WAIT_BEFORE_MOVE = 2;
        private List<Duck> ducksToMove = new List<Duck>(8);
        private List<MovementData> movementData = new List<MovementData>(8);
        private DuckMovementPresentationHandler presentationHandler;

        public DuckMovementSystem (GameplayController gameplayController) : base (gameplayController)
        {
            presentationHandler = gameplayController.GetPresentationHandler<DuckMovementPresentationHandler>();
        }

        public override void Update (float dt)
        {
            if (IsUserMoved())
                ProcessMovement();
        }

        private void ProcessMovement ()
        {
            ducksToMove.Clear();

            foreach (var duck in ducks)
            {
                duck.IncrementWaitingTurns();
                if (MustMove(duck))
                    ducksToMove.Add(duck);
            }

            if (ducksToMove.Count > 0)
                Move(ducksToMove);
        }

        private bool MustMove (Duck duck)
        {
            return
                duck.TurnsWaited() >= TURNS_TO_WAIT_BEFORE_MOVE &&
                QueryUtilities.IsFullyFree(duck.Parent()) &&
                QueryUtilities.IsGoingToBeHit(duck.Parent().Parent(), cellStackBoard) == false;
        }

        private void Move (List<Duck> ducksToMove)
        {
            movementData.Clear();

            var availableRiverCells = ExtractAvailableRiverCells(IsRiverAvailableToMoveTo);
            availableRiverCells.Shuffle();

            for (int i = 0; i < ducksToMove.Count && i < availableRiverCells.Count; ++i)
            {
                movementData.Add(new MovementData(ducksToMove[i], availableRiverCells[i]));
                ActionUtilites.Lock<DuckMovementKeyType>(ducksToMove[i].Parent());
                ActionUtilites.Lock<DuckMovementKeyType>(availableRiverCells[i].Parent());
            }

            presentationHandler.Move(movementData, FinishMovement);
        }

        private bool IsRiverAvailableToMoveTo (RiverCell riverCell)
        {
            var riverCellStack = riverCell.Parent();

            return
                QueryUtilities.IsFullyFree(riverCellStack) &&
                QueryUtilities.IsGoingToBeHit(riverCellStack, cellStackBoard) == false &&
                HasNoDuckOrDuckIsReadyToMove(riverCellStack) &&
                IsMovingRiver(riverCell) == false;
        }

        private bool HasNoDuckOrDuckIsReadyToMove (CellStack cellStack)
        {
            var duck = cellStack.HasTileStack() ? QueryUtilities.FindTile<Duck>(cellStack) : null;
            return duck == null || duck.TurnsWaited() >= TURNS_TO_WAIT_BEFORE_MOVE;
        }

        private bool IsMovingRiver (RiverCell riverCell)
        {
            return riverCell.NextRiverCell() != null;
        }

        private void FinishMovement (MovementData data)
        {
            ActionUtilites.SwapTileStacksOf(data.duck.Parent().Parent(), data.target.Parent());
            data.duck.ResetWaiting();

            ActionUtilites.Unlock(data.duck.Parent());
            ActionUtilites.Unlock(data.target.Parent());
        }
    }
}