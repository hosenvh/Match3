

using Match3.Game.Gameplay.Core;

namespace Match3.Game.Gameplay.CellAttachments
{
    public class PortalEntrance : CellAttachment
    {
        public readonly CellStack exitCellStack;

        public PortalEntrance(CellStack exitCellStack)
        {
            this.exitCellStack = exitCellStack;
        }
    }

}