using Match3.Game.Gameplay.CellAttachments;
using Match3.Game.Gameplay.Core;
using Match3.Presentation.Gameplay.Core;
using System;
using System.Collections.Generic;
using Match3.Foundation.Base.EventManagement;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Game.Gameplay.SubSystems.General;
using Match3.Game.Gameplay.TileGeneration;
using UnityEngine;
using UnityEngine.UI;


namespace Match3.Presentation.Gameplay.CellAttachments
{
    public class TileSourceCreatorPresenter : CellAttachmentPresenter, EventListener
    {
        [SerializeField] private TileSourceCreatorElementsDatabaseConfig elementsConfig = null;
        [Space]
        [SerializeField] private Transform itemsGrid = null;
        [SerializeField] private Image samplePrefab = null;
        [SerializeField] private Animation onTileCreationAnimation = null;

        private TileSourceCreator tileSourceCreator;

        protected override void InternalSetup(CellAttachment attachment)
        {
            tileSourceCreator = attachment.As<TileSourceCreator>();

            tileSourceCreator.AddComponent(this);
            SetupVisual(tileSourceCreator.GetSourceTypes());

            ServiceLocator.Find<EventManager>().Register(this);
        }

        private void SetupVisual(IEnumerable<Type> types)
        {
            foreach (Type type in types)
                Instantiate(samplePrefab, itemsGrid).sprite = elementsConfig.GetVisual(type);
        }

        public void OnEvent(GameEvent evt, object sender)
        {
            if (evt is TileStackGeneratedEvent tileStackGeneratedEvent && sender is TileSourceSystem &&
                tileStackGeneratedEvent.tileStack == tileSourceCreator.Owner().CurrentTileStack())
                PlayTileCreationEffect();
        }

        private void PlayTileCreationEffect()
        {
            onTileCreationAnimation.Stop();
            onTileCreationAnimation.Play();
        }

        private void OnDestroy()
        {
            ServiceLocator.Find<EventManager>().UnRegister(this);
        }
    }
}