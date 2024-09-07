using Match3.Foundation.Base.EventManagement;
using Match3.Game.Gameplay.Core;

namespace Match3.Game.Gameplay.SubSystems.General
{
    public struct CellReorderedEvent : GameEvent
    {
        public readonly CellStack cellStack;
        public readonly Cell cell;

        public CellReorderedEvent(CellStack cellStack, Cell cell)
        {
            this.cellStack = cellStack;
            this.cell = cell;
        }
    }
}