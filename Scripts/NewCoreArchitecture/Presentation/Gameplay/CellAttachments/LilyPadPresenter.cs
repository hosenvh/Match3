

using Match3.Game.Gameplay.CellAttachments;
using Match3.Game.Gameplay.Core;
using Match3.Presentation.Gameplay.Core;

namespace Match3.Presentation.Gameplay.CellAttachments
{
    public class LilyPadPresenter : CellAttachmentPresenter
    {

        protected override void InternalSetup(CellAttachment attachment)
        {
            attachment.As<LilyPad>().AddComponent(this);
        }
    }
}