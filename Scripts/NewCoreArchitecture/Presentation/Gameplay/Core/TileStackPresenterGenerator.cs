using Match3.Foundation.Base.EventManagement;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.Physics;
using Match3.Game.Gameplay.SubSystems.General;
using Match3.Presentation.Gameplay.Core;
using System;
using UnityEngine;

namespace Match3.Presentation.Gameplay
{
    public class TileStackPresenterGenerator : MonoBehaviour, EventListener
    {
        BoardPresenterNew boardPresenter;


        public void Setup(BoardPresenterNew boardPresenter)
        {
            this.boardPresenter = boardPresenter;
            ServiceLocator.Find<EventManager>().Register(this);
        }

        void OnDestroy()
        {
            ServiceLocator.Find<EventManager>().UnRegister(this);
        }

        public void OnEvent(GameEvent evt, object sender)
        {
            if (evt is TileStackGeneratedEvent)
                boardPresenter.CreateTileStackPresenterFor(evt.As<TileStackGeneratedEvent>().tileStack);
            else if (evt is TileGeneratedEvent)
                boardPresenter.CreateTilePresenterFor(evt.As<TileGeneratedEvent>().tile, evt.As<TileGeneratedEvent>().tileStack);

        }
    }
}