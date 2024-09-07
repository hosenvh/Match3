using System;
using System.Collections.Generic;
using Match3.Game.Gameplay.CellAttachments;
using Match3.Game.Gameplay.Cells;
using Match3.Game.Gameplay.Core;
using UnityEngine;

namespace Match3.Game.Gameplay.Physics
{

    public partial class PhysicsSystem
    {
        public class DownwardFlowCalculator
        {
            public enum FlowStatue { Undetermined, HasFlow, Blocked};

            CellStack[] cellStacks;
            CellStack[] cellStackLogicalTops;
            FlowStatue[] flowStatuses;


            PhysicsSystem system;
            int size;

            public void Setup(PhysicsSystem system)
            {
                this.system = system;
                size = system.gameBoard.size;

                SetupDownwardAvailabilityGrid();
            }

            private void SetupDownwardAvailabilityGrid()
            {
                cellStacks = system.gameBoard.leftToRightTopDownCellStackArray;
                cellStackLogicalTops = new CellStack[size];
                flowStatuses = new FlowStatue[size];


                foreach (var element in new LeftToRightTopDownGridIterator<CellStack>(system.cellStackBoard))
                {
                    var linearIndex = system.cellStackBoard.PositionToLinearIndex(element.x, element.y);
                    if (element.value.HasAttachment<PortalExit>())
                        cellStackLogicalTops[linearIndex] = element.value.GetAttachment<PortalExit>().entranceCellStack;
                    else
                        cellStackLogicalTops[linearIndex] = system.cellStackBoard.TopOf(element.x, element.y);
                    flowStatuses[linearIndex] = FlowStatue.Undetermined;
                }

            }

            public bool IsDownwardFlowStoppedFor(Vector2Int positon)
            {
                var index = system.cellStackBoard.PositionToLinearIndex(positon.x, positon.y);
                DetermineFlowValueFor(index);
                return flowStatuses[index] == FlowStatue.Blocked;
            }

            public void Clear()
            {
                for (int i = 0; i < size; ++i)
                    flowStatuses[i] = FlowStatue.Undetermined;

            }

            private void DetermineFlowValueFor(int i)
            {
                if (flowStatuses[i] != FlowStatue.Undetermined)
                    return;

                var cellStack = cellStacks[i];

                if (system.downwardBlockedCells.Contains(cellStack))
                    flowStatuses[i] = FlowStatue.Blocked;
                else if (cellStack.HasAttachment<TileSource>())
                    flowStatuses[i] = FlowStatue.HasFlow;
                else if (QueryUtilities.HasCellOnTop<HedgeCell>(cellStack) && cellStack.HasAttachment<PortalExit>()) // TODO: Try to find the right abstraction for this condition.
                    flowStatuses[i] = FlowStatue.Blocked;
                else
                {
                    var topCellStack = cellStackLogicalTops[i];
                    if (topCellStack == null)
                        flowStatuses[i] = FlowStatue.Blocked;

                    else if (system.CanTileFallThrough(topCellStack))
                    {
                        var topIndex = system.cellStackBoard.PositionToLinearIndex(topCellStack.Position().x, topCellStack.Position().y);
                        DetermineFlowValueFor(topIndex);
                        flowStatuses[i] = flowStatuses[topIndex];
                    }
                    else
                        flowStatuses[i] = system.DoesBlockFlow(topCellStack) ? FlowStatue.Blocked : FlowStatue.HasFlow;
                }

            }


        }
    }
}