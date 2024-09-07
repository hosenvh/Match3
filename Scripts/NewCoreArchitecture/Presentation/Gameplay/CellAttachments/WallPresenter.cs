using Match3.Game.Gameplay.CellAttachments;
using Match3.Game.Gameplay.Core;
using Match3.Presentation.Gameplay.Core;
using UnityEngine;

namespace Match3.Presentation.Gameplay.CellAttachments
{
    public class WallPresenter : CellAttachmentPresenter
    {

        public Transform imageTransform;


        protected override void InternalSetup(CellAttachment attachment)
        {
            if (attachment.As<Wall>().placement == GridPlacement.Down)
            {
                var rotation = imageTransform.transform.localEulerAngles;
                rotation.z = -90;
                imageTransform.transform.localEulerAngles = rotation;
             }
        }
    }
}