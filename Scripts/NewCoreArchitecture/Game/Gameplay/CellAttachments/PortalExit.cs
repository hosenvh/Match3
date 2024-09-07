

using Match3.Game.Gameplay.Core;

namespace Match3.Game.Gameplay.CellAttachments
{
    public class PortalExit : CellAttachment
    {
        public readonly CellStack entranceCellStack;

        public PortalExit(CellStack entranceCellStack)
        {
            this.entranceCellStack = entranceCellStack;
        }
    }

}