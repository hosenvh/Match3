using Match3.Foundation.Base.EventManagement;
using Match3.Game.Gameplay.Core;

namespace Match3.Game.Gameplay.SubSystems.General
{
    public struct CellGeneratedEvent : GameEvent
    {
        public readonly CellStack cellStack;
        public readonly Cell cell;

        public CellGeneratedEvent(CellStack cellStack, Cell cell)
        {
            this.cellStack = cellStack;
            this.cell = cell;
        }
    }
}