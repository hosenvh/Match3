using Match3.Game.Gameplay.CellAttachments;
using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.Tiles;
using System.Collections.Generic;

namespace Match3.Game.Gameplay.SubSystems.ButterflyMechanic
{
    public class PotentialToMoveButterflyGroupDeterminer
    {
        List<ButterflyMovementGroup> potentialGroups = new List<ButterflyMovementGroup>();
        HashSet<CellStack> processButterfliesCellStacks = new HashSet<CellStack>();
        GameBoard gameBoard;

        CellStack[] cellStackLogicalTops;
        CellStack[] cellStackLogicalBottoms;

        public PotentialToMoveButterflyGroupDeterminer(GameBoard gameBoard)
        {
            this.gameBoard = gameBoard;

            InitialLogicalCellStackTopsAndBottoms();
        }

        private void InitialLogicalCellStackTopsAndBottoms()
        {
            this.cellStackLogicalTops = new CellStack[gameBoard.size];
            this.cellStackLogicalBottoms = new CellStack[gameBoard.size];

            foreach (var cellStack in gameBoard.ArrbitrayCellStackArray())
            {
                var pos = cellStack.Position();
                var linearIndex = gameBoard.CellStackBoard().PositionToLinearIndex(pos.x, pos.y);

                if (cellStack.HasAttachment<PortalExit>())
                    cellStackLogicalTops[linearIndex] = cellStack.GetAttachment<PortalExit>().entranceCellStack;
                else
                    cellStackLogicalTops[linearIndex] = gameBoard.CellStackBoard().TopOf(pos.x, pos.y);

                if (cellStack.HasAttachment<PortalEntrance>())
                    cellStackLogicalBottoms[linearIndex] = cellStack.GetAttachment<PortalEntrance>().exitCellStack;
                else
                    cellStackLogicalBottoms[linearIndex] = gameBoard.CellStackBoard().BottomOf(pos.x, pos.y);
            }
        }

        public List<ButterflyMovementGroup> Determine()
        {
            DeterminePotentialToMoveButterflies();
            return potentialGroups;
        }

        private void DeterminePotentialToMoveButterflies()
        {
            potentialGroups.Clear();
            processButterfliesCellStacks.Clear();

            foreach (var cellStack in gameBoard.ArrbitrayCellStackArray())
            {
                if(HasButterfly(cellStack) && HasButterfly(BottomOf(cellStack)) == false)
                {
                    var group = new ButterflyMovementGroup(new List<TileStack>());
                    var currentCellStack = cellStack;

                    while (HasButterfly(currentCellStack))
                    {
                        group.Add(currentCellStack.CurrentTileStack());
                        processButterfliesCellStacks.Add(currentCellStack);
                        currentCellStack = TopOf(currentCellStack);
                    }

                    var top = TopOf(group.Head().Parent());
                    if (CanMoveTo(top))
                    {
                        group.topTileStack = top.CurrentTileStack();
                        potentialGroups.Add(group);
                    }
                }
            }
        }

        bool HasButterfly(CellStack cellStack)
        {
            return cellStack != null
                && QueryUtilities.HasTileOnTop<Butterfly>(cellStack.CurrentTileStack());
        }

        bool CanMoveTo(CellStack cellStack)
        {
            if (cellStack == null || cellStack.Top().CanContainTile() == false || cellStack.HasTileStack() == false || cellStack.CurrentTileStack().IsDepleted())
                return false;
            return cellStack.CurrentTileStack().Top().GetComponent<Physics.TilePhysicalProperties>().isAffectedByGravity == true;
        }

        CellStack TopOf(CellStack cellStack)
        {
            return cellStackLogicalTops[gameBoard.CellStackBoard().PositionToLinearIndex(cellStack.Position().x, cellStack.Position().y)];
        }

        CellStack BottomOf(CellStack cellStack)
        {
            return cellStackLogicalBottoms[gameBoard.CellStackBoard().PositionToLinearIndex(cellStack.Position().x, cellStack.Position().y)];
        }
    }
}