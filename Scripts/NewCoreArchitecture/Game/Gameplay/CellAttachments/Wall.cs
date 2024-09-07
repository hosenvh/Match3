using Match3.Game.Gameplay.Core;

namespace Match3.Game.Gameplay.CellAttachments
{
    public class Wall : CellAttachment
    {
        public readonly GridPlacement placement;


        public Wall(GridPlacement placement)
        {
            this.placement = placement;

        }
    }
}