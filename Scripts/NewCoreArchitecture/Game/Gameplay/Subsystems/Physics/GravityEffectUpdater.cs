using System.Linq;
using System.Collections.Generic;
using Match3.Game.Gameplay.CellAttachments;
using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.SubSystemsData.SessionData;
using System;

namespace Match3.Game.Gameplay.Physics
{
    public partial class PhysicsSystem
    {
        public class GravityEffectUpdater
        {
            PhysicsSystem system;

            bool shouldForceStablize;

            CellStack[] cellStacks;
            CellStack[] cellStackLogicalBottoms;
            CellStack[] cellStackBottomLefts;
            CellStack[] cellStackBottomRights;

            HashSet<CellStack> permanentLeftSideFallBlockedCells = new HashSet<CellStack>();
            HashSet<CellStack> permanentRightSideFallBlockedCells = new HashSet<CellStack>();

            int size;
            int width;
            int height;

            CellStackBoard cellStackBoard;

            BoardBlockageController blockageControllers;

            public void Setup(PhysicsSystem system)
            {
                this.system = system;
                this.cellStackBoard = system.cellStackBoard;
                this.blockageControllers = system.gameplayController.boardBlockageController;


                width = cellStackBoard.Width();
                height = cellStackBoard.Height();

                cellStacks = system.gameBoard.leftToRightButtomUpCellStackArray;
                size = cellStacks.Length;

                cellStackLogicalBottoms = new CellStack[size];
                cellStackBottomLefts = new CellStack[size];
                cellStackBottomRights = new CellStack[size];


                SetupDirectionalArrays();
                SetupBlockages();
            }


            void SetupDirectionalArrays()
            {
                int i = 0;
                foreach (var element in new LeftToRightBottomUpGridIterator<CellStack>(system.gameBoard.CellStackBoard()))
                {
                    SetupTilePhysic(element.value);

                    if (element.value.HasAttachment<PortalEntrance>())
                        cellStackLogicalBottoms[i] = element.value.GetAttachment<PortalEntrance>().exitCellStack;
                    else
                        cellStackLogicalBottoms[i] = system.cellStackBoard.BottomOf(element.x, element.y);

                    cellStackBottomLefts[i] = cellStackBoard[element.x - 1, element.y + 1];
                    cellStackBottomRights[i] = cellStackBoard[element.x + 1, element.y + 1];

                    i++;
                }
            }


            private void SetupBlockages()
            {
                for (int i = 0; i < cellStacks.Length; ++i)
                    SetupBlockageFor(i);
            }

            private void SetupBlockageFor(int i)
            {
                var cellStack = cellStacks[i];

                if (blockageControllers.IsBlocked(cellStack, Direction.Down))
                    if (cellStackLogicalBottoms[i] != null)
                        system.downwardBlockedCells.Add(cellStackLogicalBottoms[i]);

                if (blockageControllers.CanNotSideFallFromTopLeft(cellStack))
                    permanentRightSideFallBlockedCells.Add(cellStack);

                if (blockageControllers.CanNotSideFallFromTopRight(cellStack))
                    permanentLeftSideFallBlockedCells.Add(cellStack);
            }

            public void UpdatePhysicBlockageFor(CellStack cellStack)
            {
                var cellStackIndex = IndexOf(cellStack);

                if (cellStackLogicalBottoms[cellStackIndex] != null)
                    system.downwardBlockedCells.Remove(cellStackLogicalBottoms[cellStackIndex]);

                permanentRightSideFallBlockedCells.Remove(cellStack);
                permanentLeftSideFallBlockedCells.Remove(cellStack);

                SetupBlockageFor(cellStackIndex);
            }

            public void SetupTilePhysic(CellStack cellStack)
            {
                if (cellStack.HasTileStack())
                    ResetPhysicalState(cellStack.CurrentTileStack().componentCache.tileStackPhysicalState);
            }

            public bool CalculateCanTileStackFallThrough(CellStack cellStack)
            {
                return cellStack.HasTileStack() == false && TryFindFirstDownwardValidCellToMove(IndexOf(cellStack)) != null;
            }

            private int IndexOf(CellStack cellStack)
            {
                return (height - cellStack.Position().y - 1) * width + cellStack.Position().x;
            }

            public void Update()
            {
                shouldForceStablize = system.GetSessionData<StabilityControlData>().shouldForceStablize;
                foreach (var tileStack in system.GetFrameData<GeneratedTileStacksData>().tileStacks)
                    SetupTileStackMoving(tileStack, tileStack.Parent(), tileStack.Parent());

                for (int i = 0; i < size; ++i)
                    UpdateGravityEffectOf(i);
            }

            private void UpdateGravityEffectOf(int i)
            {
                var currentCell = cellStacks[i];
                if (ShoudNotProcessGravityEffect(currentCell))
                    return;

                var tileStack = currentCell.CurrentTileStack();
                var tileState = tileStack.componentCache.tileStackPhysicalState;

                if (IsFallingTowardATarget(tileState))
                    return;


                CellStack targetCellStack = TryFindFirstValidCellStackToMove(i);

                if (targetCellStack != null)
                    SetupTileStackMoving(tileStack, currentCell, targetCellStack);
                else if (tileState.PhysicsState() == PhysicsState.Falling)
                    TryStopTileStackMoving(tileStack, currentCell);
            }


            void SetupTileStackMoving(TileStack tileStack, CellStack origin, CellStack destination)
            {
                origin.DetachTileStack();
                destination.SetCurrnetTileStack(tileStack);
                tileStack.componentCache.tileStackPhysicalState.hasReachedTarget = false;
                tileStack.componentCache.tileStackPhysicalState.SetPhysicsState(PhysicsState.Falling);
                tileStack.componentCache.lockState.LockBy<PhysicsKeyType>();
                if (destination.HasAttachment<PortalExit>())
                {
                    var pos = destination.Position();
                    pos.y -= 1;
                    tileStack.SetPosition(pos);
                }

            }

            void TryStopTileStackMoving(TileStack tileStack, CellStack parentCell)
            {
                var tileState = tileStack.componentCache.tileStackPhysicalState;
                if (tileState.hasReachedTarget)
                {
                    tileStack.SetPosition(parentCell.Position());
                    tileStack.componentCache.lockState.Release();
                    ResetPhysicalState(tileState);
                }
            }

            CellStack TryFindFirstValidCellStackToMove(int i)
            {
                if (shouldForceStablize)
                    return null;

                CellStack foundCell = null;

                foundCell = TryFindFirstDownwardValidCellToMove(i);
                if (foundCell == null)
                    foundCell = TryFindSidewayValidCellToMove(i);

                return foundCell;
            }

            CellStack TryFindSidewayValidCellToMove(int i)
            {
                CellStack foundCell = null;
                var orignCellStack = cellStacks[i];

                var bottomLeftCell = cellStackBottomLefts[i];
                var bottomRightCell = cellStackBottomRights[i];

                if (IsValidForMoving(bottomLeftCell) && CanSideFallToLeft(orignCellStack, bottomLeftCell))
                    foundCell = bottomLeftCell;
                else if (IsValidForMoving(bottomRightCell) && CanSideFallToRight(orignCellStack, bottomRightCell))
                    foundCell = bottomRightCell;


                return foundCell;
            }

            CellStack TryFindFirstDownwardValidCellToMove(int i)
            {
                CellStack currentCellStack = null;

                int index = i;

                CellStack nextCell = cellStackLogicalBottoms[index];

                currentCellStack = nextCell;

                bool isValid = IsValidForDownwardFalling(currentCellStack);

                while (system.CanTileFallThrough(nextCell) && isValid == false)
                {
                    currentCellStack = nextCell;
                    isValid = IsValidForDownwardFalling(currentCellStack);
                    index = BottomIndexOf(index);

                    if (index < 0)
                        break;

                    nextCell = cellStackLogicalBottoms[index];
                }

                if (isValid)
                    return currentCellStack;
                else
                    return null;
            }

            int BottomIndexOf(int index)
            {
                // NOTE: This is dependent on direction of iteration.
                return index - width;
            }


            bool ShoudNotProcessGravityEffect(CellStack cellStack)
            {
                var tileStack = cellStack.CurrentTileStack();

                return tileStack == null
                    || tileStack.IsDepleted()
                    || tileStack.Top().componentCache.tilePhysicalProperties.isAffectedByGravity == false
                    || tileStack.componentCache.lockState.IsFreeOrLockedBy<PhysicsKeyType>() == false;
            }

            bool IsFallingTowardATarget(TileStackPhysicalState tileState)
            {
                return !tileState.hasReachedTarget && tileState.PhysicsState() == PhysicsState.Falling;
            }

            bool IsValidForDownwardFalling(CellStack cellStack)
            {
                return system.downwardBlockedCells.Contains(cellStack) == false 
                    && IsValidForMoving(cellStack);
            }

            bool IsValidForMoving(CellStack cellStack)
            {
                return
                    cellStack != null
                    && cellStack.HasTileStack() == false
                    && cellStack.Top().CanContainTile()
                    && cellStack.componentCache.lockState.IsFree();
            }

            bool CanSideFallToLeft(CellStack currentCell, CellStack sideCell)
            {
                return permanentLeftSideFallBlockedCells.Contains(sideCell) == false
                    && CanSideFall(currentCell, sideCell);
            }

            bool CanSideFallToRight(CellStack currentCell, CellStack sideCell)
            {
                return permanentRightSideFallBlockedCells.Contains(sideCell) == false
                    && CanSideFall(currentCell, sideCell);
            }

            bool CanSideFall(CellStack currentCell, CellStack sideCell)
            {
                return system.IsDownwardFlowStoppedFor(sideCell.Position()) 
                    || CanSideFallConsideringPrority(currentCell);
  
            }

            private bool CanSideFallConsideringPrority(CellStack currentCell)
            {
                return system.IsProritizeForSideFalling(currentCell.CurrentTileStack().Top())
                    && !system.IsCrossable(BottomOf(currentCell));
            }

            CellStack BottomOf(CellStack currentCell)
            {
                return cellStackBoard.BottomOf(currentCell.Position());
            }

            void ResetPhysicalState(TileStackPhysicalState state)
            {
                state.SetPhysicsState(PhysicsState.Resting);
                state.hasReachedTarget = false;
                state.speed = 0;
            }
        }
    }
}