using System;
using System.Linq;
using System.Collections.Generic;
using Match3.Game.Gameplay.CellAttachments;
using Match3.Game.Gameplay.Cells;
using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.SubSystemsData.FrameData.General;
using Match3.Game.Gameplay.SubSystemsData.SessionData;
using Match3.Game.Gameplay.SubSystemsData.FrameData.RiverMechanic;
using static Match3.Game.Gameplay.QueryUtilities;

namespace Match3.Game.Gameplay.SubSystems.RiverMechanic
{

    public interface RiverMovementPresentationHandler : PresentationHandler
    {
        void MoveRiverCells(List<RiverCell> riverCells, Action onCompleted);
    }



    class RiverKeyType : KeyType
    {

    }



    [After(typeof(General.UserMovementManagementSystem))]
    public class RiverMovementSystem : GameplaySystem
    {
        struct River
        {
            public List<RiverCell> sequence;
        }

        enum State { WaitingForUserMovement, ProcessingRiverMovement};

        List<River> flowingRivers = new List<River>();
        List<River> blockedRivers = new List<River>();

        UserMovementData userMovementData;
        State state = State.WaitingForUserMovement;
        RiverMovementPresentationHandler presentationHandler;

        List<CellAttachment> currentAttachmentsTemporaryCache = new List<CellAttachment>();
        List<CellAttachment> nextAttachmentsTemporaryCache = new List<CellAttachment>();

        public RiverMovementSystem(GameplayController gameplayController) : base(gameplayController)
        {
            ExtractRivers(gameplayController.GameBoard());

            userMovementData = GetFrameData<UserMovementData>();

            presentationHandler = gameplayController.GetPresentationHandler<RiverMovementPresentationHandler>();


        }

        public override void Reset()
        {
            if (flowingRivers.Count == 0)
                gameplayController.DeactivateSystem<RiverMovementSystem>();
        }



        public override void Update(float dt)
        {
            switch (state)
            {
                case State.WaitingForUserMovement:
                    CheckUserMovement();

                    break;
                case State.ProcessingRiverMovement:
                    ProcessRiverMovement();
                    break;
            }

        }

        private void CheckUserMovement()
        {
            if (userMovementData.moves > 0)
            {
                GetSessionData<InputControlData>().AddLockedBy<RiverKeyType>();
                state = State.ProcessingRiverMovement;
            }
        }


        private void ProcessRiverMovement()
        {
            if (CanMoveAllRivers())
            {
                ClearBlockedRiversList();
                MoveUnblockedRivers();

                if (IsAllRiversBlocked())
                    RemoveRiverLockFromInputControlData();
            }
        }

        private bool CanMoveAllRivers()
        {
            foreach (var river in flowingRivers)
                foreach (var cell in river.sequence)
                    if (IsStable(cell.Parent()) == false || IsStable(BellowOf(cell.Parent())) == false)
                        return false;

            return true;
        }

        private void ClearBlockedRiversList()
        {
            blockedRivers.Clear();
        }

        private bool IsAllRiversBlocked()
        {
            return blockedRivers.SequenceEqual(flowingRivers);
        }

        private CellStack BellowOf(CellStack cellStack)
        {
            return gameplayController.GameBoard().DirectionalCellStackOf(cellStack.Position(), Direction.Down);
        }

        private bool IsStable(CellStack cellStack)
        {
            return cellStack == null || IsFullyFree(cellStack);
        }

        private void MoveUnblockedRivers()
        {
            foreach (var river in flowingRivers)
            {
                if (IsRiverBlocked(river))
                {
                    AddToBlockedRiversList(river);
                    continue;
                }

                LockRiver(river);
                presentationHandler.MoveRiverCells(river.sequence, () => ApplyRiverMovement(river));
            }
            
            bool IsRiverBlocked(River river)
            {
                foreach (var riverCell in river.sequence)
                    if (GetFrameData<RiverBlockingData>().BlockedRiverCells.Any(cell => cell == riverCell))
                        return true;

                return false;
            }

            void AddToBlockedRiversList(River river)
            {
                blockedRivers.Add(river);
            }
        }

        private void LockRiver(River river)
        {
            foreach(var cell in river.sequence)
            {
                cell.Parent().GetComponent<LockState>().LockBy<RiverKeyType>();
                var tileStack = cell.Parent().CurrentTileStack();
                if(tileStack != null)
                    tileStack.GetComponent<LockState>().LockBy<RiverKeyType>();
            }
        }

        private void ReleaseRiver(River river)
        {
            foreach (var cell in river.sequence)
            {
                cell.Parent().GetComponent<LockState>().Release();
                var tileStack = cell.Parent().CurrentTileStack();
                if (tileStack != null)
                    tileStack.GetComponent<LockState>().Release();
            }
        }

        // TODO: Refactor this shit. 
        private void ApplyRiverMovement(River river)
        {

            TileStack nextTileStack = null;
            TileStack currentTileStack = null;

            nextTileStack = river.sequence[0].Parent().CurrentTileStack();

            ClearAndAdd(nextAttachmentsTemporaryCache, river.sequence[0].Parent().Attachments());

            foreach (var cell in river.sequence)
            {
                var nextCell = cell.NextRiverCell();


                currentTileStack = nextTileStack;
                nextTileStack = nextCell.Parent().CurrentTileStack();

                ClearAndAdd(currentAttachmentsTemporaryCache, nextAttachmentsTemporaryCache);
                ClearAndAdd(nextAttachmentsTemporaryCache, nextCell.Parent().Attachments());

                if (currentTileStack != null)
                {
                    nextCell.Parent().SetCurrnetTileStack(currentTileStack);
                    currentTileStack.SetPosition(nextCell.Parent().Position());
                }
                else
                    nextCell.Parent().DetachTileStack();


                foreach (var attachment in nextAttachmentsTemporaryCache)
                    if (ShouldMove(attachment))
                        nextCell.Parent().RemoveAttachment(attachment);

                foreach (var attachment in currentAttachmentsTemporaryCache)
                    if (ShouldMove(attachment))
                        nextCell.Parent().Attach(attachment);
            }

            ReleaseRiver(river);
            RemoveRiverLockFromInputControlData();
        }

        private void RemoveRiverLockFromInputControlData()
        {
            GetSessionData<InputControlData>().RemoveLockedBy<RiverKeyType>();
            state = State.WaitingForUserMovement;
        }

        private void ClearAndAdd(List<CellAttachment> target ,IEnumerable<CellAttachment> attachments)
        {
            target.Clear();
            target.AddRange(attachments);
        }

        private bool ShouldMove(CellAttachment attachment)
        {
            return attachment is LilyPad || attachment is LilyPadBud;
        }

        private void ExtractRivers(GameBoard gameBoard)
        {
            HashSet<RiverCell> processedCells = new HashSet<RiverCell>();
            foreach (var element in gameBoard.DefaultCellBoardIterator())
            {
                var riverCell = TryGetRiverCellIn(element.value);
                if (riverCell != null && processedCells.Contains(riverCell) == false)
                {
                    ExtractRiverFrom(riverCell, ref processedCells);
                }
            }
        }

        private void ExtractRiverFrom(RiverCell riverCell, ref HashSet<RiverCell> processedCells)
        {
            var river = new River();
            river.sequence = new List<RiverCell>();


            var currentRiverCell = riverCell;
            bool shouldFlow = riverCell.NextRiverCell() != null;

            do
            {
                processedCells.Add(currentRiverCell);
                river.sequence.Add(currentRiverCell);
                currentRiverCell = currentRiverCell.NextRiverCell();

            } while ((currentRiverCell != null && processedCells.Contains(currentRiverCell) == false));

            if(shouldFlow)
                flowingRivers.Add(river);

        }

        private RiverCell TryGetRiverCellIn(CellStack cellStack)
        {
            foreach (var cell in cellStack.Stack())
                if (cell is RiverCell)
                    return cell as RiverCell;
            return null;
        }
    }
 }
