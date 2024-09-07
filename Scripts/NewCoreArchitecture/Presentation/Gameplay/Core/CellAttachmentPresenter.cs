using Match3.Game.Gameplay.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Match3.Presentation.Gameplay.Core
{
    public abstract class CellAttachmentPresenter : MonoBehaviour, Match3.Foundation.Base.ComponentSystem.Component
    {
        CellAttachment attachment;


        public void Setup(CellAttachment attachment)
        {
            this.attachment = attachment;
            attachment.AddComponent(this);
            InternalSetup(attachment);
        }

        protected abstract void InternalSetup(CellAttachment attachment);

        public CellAttachment Attachment()
        {
            return attachment;
        }
    }
}