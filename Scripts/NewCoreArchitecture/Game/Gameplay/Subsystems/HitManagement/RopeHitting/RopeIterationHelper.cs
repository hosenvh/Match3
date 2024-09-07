
using Match3.Game.Gameplay.CellAttachments;
using Match3.Game.Gameplay.Core;
using System;
using System.Collections.Generic;

namespace Match3.Game.Gameplay.HitManagement
{
    public partial class RopeHitGenerator
    {
        private class RopeIterationHelper
        {
            CellStackBoard cellStackBoard;
            List<Rope> ropesTemp = new List<Rope>();

            public RopeIterationHelper(CellStackBoard cellStackBoard)
            {
                this.cellStackBoard = cellStackBoard;
            }

            // NOTE: Here it is assumed rope is only place in left or bottom of the cellStack.
            // TODO: It's better to move this assumption to another place.
            public void ForeachRopesArroundHitDo(Action<Rope> action, CellStack center)
            {
                foreach (var rope in ExtractRopesFrom(center))
                    action.Invoke(rope);

                foreach (var rope in ExtractRopesFrom(TopOf(center)))
                    if (rope.placement == GridPlacement.Down)
                        action.Invoke(rope);

                foreach (var rope in ExtractRopesFrom(RightOf(center)))
                    if (rope.placement == GridPlacement.Left)
                        action.Invoke(rope);
            }


            private List<Rope> ExtractRopesFrom(CellStack cellStack)
            {
                ropesTemp.Clear();

                if (cellStack != null)
                    cellStack.GetAttachments<Rope>(ref ropesTemp);

                return ropesTemp;
            }

            private CellStack TopOf(CellStack cellStack)
            {
                return cellStackBoard.DirectionalElementOf(cellStack.Position(), Direction.Up);
            }

            private CellStack RightOf(CellStack cellStack)
            {
                return cellStackBoard.DirectionalElementOf(cellStack.Position(), Direction.Right);
            }
        }
    }
}