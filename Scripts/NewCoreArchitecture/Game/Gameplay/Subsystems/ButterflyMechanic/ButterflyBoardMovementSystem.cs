

using Match3.Game.Gameplay.CellAttachments;
using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.SubSystemsData.FrameData;
using Match3.Game.Gameplay.SubSystemsData.SessionData;
using Match3.Game.Gameplay.Tiles;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Match3.Game.Gameplay.SubSystems.ButterflyMechanic
{

    public struct ButterflyMovementKeyType : KeyType{ }

    public interface ButterflyMovementPresentationHandler : PresentationHandler
    {
        void Move(List<ButterflyMovementGroup> movementGroups, Action<ButterflyMovementGroup> onMoveCompleted, Action onCompleted);
    }

    [After(typeof(General.UserMovementManagementSystem))]
    [After(typeof(TileGeneration.TileSourceSystem))]
    public class ButterflyBoardMovementSystem : GameplaySystem
    {
        public enum State { WaitingForUserMovement, ProcessingButterfliesMovement, WaitingForMovementToFinish };


        bool justFinishedButterflyMovement;
        State state;

        StabilityData stabilityData;
        PotentialToMoveButterflyGroupDeterminer potentialGroupDeterminer;
        ButterflyMovementPresentationHandler presentationHandler;

        // TODO: Try to find a better approach.

        public ButterflyBoardMovementSystem(GameplayController gameplayController) : base(gameplayController)
        {
            justFinishedButterflyMovement = false;
            this.stabilityData = GetFrameData<StabilityData>();
            this.presentationHandler = gameplayController.GetPresentationHandler<ButterflyMovementPresentationHandler>();

            potentialGroupDeterminer = new PotentialToMoveButterflyGroupDeterminer(gameplayController.GameBoard());
        }

        public override void OnActivated()
        {
            state = State.WaitingForUserMovement;
            justFinishedButterflyMovement = false;
        }

        public override void Update(float dt)
        {
            // NOTE: This must happen before the state processing.
            justFinishedButterflyMovement = false;

            switch (state)
            {
                case State.ProcessingButterfliesMovement:
                    ProcessButterfliesMovement();
                    break;
            }
        }

        void ProcessButterfliesMovement()
        {
            if (ThereIsNotButterflyInBoardThatNeedsMovement())
                FinishMovementState();
            else if (stabilityData.wasStableLastChecked && gameplayController.boardStabilityCalculator.CalculateIsStable())
            {
                stabilityData.wasStableLastChecked = false;

                var butterflyGroups = potentialGroupDeterminer.Determine();

                if (butterflyGroups.Count == 0)
                    FinishMovementState();
                else if (AllPotentialButterfliesReadyToMove(butterflyGroups))
                {
                    MoveAllButterflies(butterflyGroups);
                    state = State.WaitingForMovementToFinish;
                }
            }
        }

        private bool ThereIsNotButterflyInBoardThatNeedsMovement()
        {
            foreach (var cellStack in gameplayController.GameBoard().ArrbitrayCellStackArray())
                if (QueryUtilities.HasTileOnTop<Butterfly>(cellStack.CurrentTileStack()))
                    return false;

            return true;
        }

        bool AllPotentialButterfliesReadyToMove(List<ButterflyMovementGroup> butterflyGroups)
        {
            foreach (var group in butterflyGroups)
            {
                if(group.topTileStack.GetComponent<LockState>().IsFree() == false)
                    return false;

                foreach(var tileStack in group.butterfliesTilestacks)
                    if (tileStack.GetComponent<LockState>().IsFree() == false)
                        return false;
            }
            return true;
        }

        void MoveAllButterflies(List<ButterflyMovementGroup> butterflyGroups)
        {
            foreach (var group in butterflyGroups)
            {
                group.topTileStack.GetComponent<LockState>().LockBy<ButterflyMovementKeyType>();

                foreach (var tileStack in group.butterfliesTilestacks)
                    tileStack.GetComponent<LockState>().LockBy<ButterflyMovementKeyType>();
            }

            presentationHandler.Move(
                butterflyGroups,
                ApplyGroupMovement,
                FinishMovementState
                );
        }

        void ApplyGroupMovement(ButterflyMovementGroup group)
        {
            var tailCellStack = group.Tail().Parent();

            for (int i = 0; i < group.butterfliesTilestacks.Count - 1; ++i)
                ApplyTileStackMovement(group.butterfliesTilestacks[i], group.butterfliesTilestacks[i + 1].Parent());

            ApplyTileStackMovement(group.Head(), group.topTileStack.Parent());
            ApplyTileStackMovement(group.topTileStack, tailCellStack);
        }

        void ApplyTileStackMovement(TileStack tileStack, CellStack target)
        {
            target.SetCurrnetTileStack(tileStack);
            tileStack.SetPosition(target.Position());
            tileStack.GetComponent<LockState>().Release();
        }

        void FinishMovementState()
        {
            state = State.WaitingForUserMovement;
            justFinishedButterflyMovement = true;
            GetSessionData<InputControlData>().RemoveLockedBy<ButterflyMovementKeyType>();
        }


        public State CurrentState()
        {
            return state;
        }

        public void GoToState(State state)
        {
            this.state = state;
        }

        public bool JustFinishedButterflyMovement()
        {
            return justFinishedButterflyMovement;
        }
    }
}