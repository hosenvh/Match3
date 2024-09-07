using KitchenParadise.Utiltiy.Base;
using Match3.Game.Gameplay.CellAttachments;
using Match3.Game.Gameplay.Cells;
using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.SubSystemsData.FrameData.General;
using Match3.Game.Gameplay.SubSystemsData.SessionData;
using Match3.Game.Gameplay.Tiles;
using System;
using System.Collections.Generic;

namespace Match3.Game.Gameplay.SubSystems.BuoyantMechanic
{
    public interface BuoyantMovementPort : PresentationHandler
    {
        void Move(List<BuoyantMovementData> movementsData, Action<BuoyantMovementData> onCompleted);
    }

    public interface BuoyantMovementSystemKeyType : KeyType
    {

    }

    public struct BuoyantMovementData
    {
        public readonly Buoyant buoyant;
        public readonly RiverCell target;

        public BuoyantMovementData(Buoyant buoyant, RiverCell target)
        {
            this.buoyant = buoyant;
            this.target = target;
        }
    }

    [After(typeof(DestructionManagement.DestructionSystem))]
    [After(typeof(General.UserMovementManagementSystem))]
    public class BuoyantMovementSystem : BuoyantSystem
    {
        const int TURNS_TO_WAIT_BEFORE_MOVE = 2;

        List<Buoyant> buoyantsToMove = new List<Buoyant>(8);

        List<BuoyantMovementData> movementsData = new List<BuoyantMovementData>(8);
        BuoyantMovementPort movementPort;

        public BuoyantMovementSystem(GameplayController gameplayController) : base(gameplayController)
        {
        }

        public override void Start()
        {
            movementPort = gameplayController.GetPresentationHandler<BuoyantMovementPort>();
        }

        public override void Update(float dt)
        {
            UpdateBuoyantsForDestoryedObjects();

            if (IsUserMoved())
                ProcessMovement();

            UpdateBuoyantsForGeneratedObjects();
        }

        private void UpdateBuoyantsForDestoryedObjects()
        {
            foreach (var tile in destrotedObjectsData.tiles)
                if (tile is Buoyant buoyant)
                    buoyants.Remove(buoyant);
        }

        private void UpdateBuoyantsForGeneratedObjects()
        {
            foreach (var tile in generatedObjectsData.tiles)
                if (tile is Buoyant buoyant)
                    buoyants.Add(buoyant);
        }

        void ProcessMovement()
        {
            buoyantsToMove.Clear();

            foreach (var buoyant in buoyants)
            {
                buoyant.IncrementWaitingTurns();
                if (MustMove(buoyant))
                    buoyantsToMove.Add(buoyant);
            }

            if (buoyantsToMove.Count > 0)
                Move(buoyantsToMove);
        }

        private bool MustMove(Buoyant buoyant)
        {
            return
                buoyant.TurnsWaited() >= TURNS_TO_WAIT_BEFORE_MOVE &&
                QueryUtilities.IsFullyFree(buoyant.Parent()) &&
                QueryUtilities.IsGoingToBeHit(buoyant.Parent().Parent(), cellStackBoard) == false;
        }

        private void Move(List<Buoyant> buoyantsToMove)
        {
            movementsData.Clear();

            var availableRiverCells = ExtractAvailableRiverCells(IsRiverAvailableToMoveTo);
            availableRiverCells.Shuffle();


            for (int i = 0; i < buoyantsToMove.Count && i < availableRiverCells.Count; ++i)
            {
                movementsData.Add(new BuoyantMovementData(buoyantsToMove[i], availableRiverCells[i]));
                ActionUtilites.Lock<BuoyantMovementSystemKeyType>(buoyantsToMove[i].Parent());
                ActionUtilites.Lock<BuoyantMovementSystemKeyType>(availableRiverCells[i].Parent());
            }

            movementPort.Move(movementsData, FinishMovement);
        }

        // TODO: Should check tilestack being empty for  the target?
        private bool IsRiverAvailableToMoveTo(RiverCell riverCell)
        {
            var riverCellStack = riverCell.Parent();
            return
                QueryUtilities.IsFullyFree(riverCellStack) &&
                QueryUtilities.IsGoingToBeHit(riverCellStack, cellStackBoard) == false &&
                HasNoBuoyatOrBuoyantIsReadyToMove(riverCellStack) &&
                IsMovingRiver(riverCell) == false;
        }

        private bool HasNoBuoyatOrBuoyantIsReadyToMove(CellStack cellStack)
        {
            var buoyant = cellStack.HasTileStack()? QueryUtilities.FindTile<Buoyant>(cellStack) : null;
            return buoyant == null || buoyant.TurnsWaited() >= TURNS_TO_WAIT_BEFORE_MOVE;
        }

        private bool IsMovingRiver(RiverCell riverCell)
        {
            return riverCell.NextRiverCell() != null;
        }


        private void FinishMovement(BuoyantMovementData data)
        {
            SwapTileStacksOf(data.buoyant.Parent().Parent(), data.target.Parent());
            data.buoyant.ResetWaiting();
            ActionUtilites.Unlock(data.buoyant.Parent());
            ActionUtilites.Unlock(data.target.Parent());
        }

        // TODO: Move this to something like ActionUtilities later.
        private void SwapTileStacksOf(CellStack cellStack1, CellStack cellStack2)
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

            if (cellStack1.HasTileStack())
                cellStack1.CurrentTileStack().SetPosition(cellStack1.Position());
            if (cellStack2.HasTileStack())
                cellStack2.CurrentTileStack().SetPosition(cellStack2.Position());
        }
    }
}