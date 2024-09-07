using Match3.Foundation.Base.EventManagement;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.Physics;
using Match3.Game.Gameplay.Tiles;
using Match3.Presentation.Gameplay.PrefabDatabases;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Match3.Presentation.Gameplay.Core
{

    public class TileStackPresenter : MonoBehaviour,  IPointerDownHandler, IDragHandler, Foundation.Base.ComponentSystem.Component
    {

        public RectTransform contentTransform;
        public AnimationPlayer animationPlayer;

        public TileStack tileStack;

        public UnityEvent playBounceEffect;

        TileStackPresenterContainer container;


        bool shouldUpdateToLogicalPositon = true;

        GameplaySoundManager soundManager;


        public void Setup(TileStack tileStack, ref TileStackPresenterContainer container, TilePresenterFactory tilePresenterFactory)
        {
            this.tileStack = tileStack;
            this.container = container;
            container.Add(this);

            tileStack.AddComponent(this);
            tileStack.onDestroyed += OnTileStackDestroy;

            soundManager = ServiceLocator.Find<GameplaySoundManager>();

            // TODO: Add a properties to tiles for this.
            if (tileStack.IsDepleted() == false && tileStack.Top() is Sand == false)
                tileStack.GetComponent<TileStackPhysicalState>().onPhysicalStateChanged += onPhysicsStateChanged;

            var tiles = new List<Tile>(tileStack.Stack());
            tiles.Reverse();

            foreach (var tile in tiles)
                SetupTilePresentationFor(tile, tilePresenterFactory);
        }

        private void onPhysicsStateChanged(PhysicsState state)
        {
            if (state == PhysicsState.Resting)
            {
                animationPlayer.Play("Bounce");
                //playBounceEffect.Invoke();
                soundManager.PlayTileStopSoundEffect();
            }

        }

        void OnDestroy()
        {
            tileStack.onDestroyed -= OnTileStackDestroy;
            container.Remove(this);
        }

        private void OnTileStackDestroy()
        {
            Destroy(this.gameObject);
        }

        public void SetupTilePresentationFor(Tile tile, TilePresenterFactory tilePresenterFactory)
        {
            tilePresenterFactory.CreateFor(tile, contentTransform).Setup(tile, this);                
        }

        public void UpdatePosition(BoardDimensions boardDimensions)
        {
            if (tileStack == null)
                return;

            if (shouldUpdateToLogicalPositon == false)
                return;

            UpdateLocalPosition(boardDimensions);
        }

        private void UpdateLocalPosition(BoardDimensions boardDimensions)
        {
            this.transform.localPosition = boardDimensions.LogicalPosToPresentaionalPos(tileStack.Position());
        }

        // TODO: Some optimization can be done here (e.g. send event when tile is selected).
        public void OnDrag(PointerEventData eventData)
        {
            ServiceLocator.Find<EventManager>().Propagate(
                new TileStackPresenterDraggedEvent(this, eventData),
                this);

        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if(eventData.button == PointerEventData.InputButton.Left)
                ServiceLocator.Find<EventManager>().Propagate(new TileStackPresenterClickedEvent(this), this);
            else if(eventData.button == PointerEventData.InputButton.Right)
                ServiceLocator.Find<EventManager>().Propagate(new TileStackPresenterRightClickedEvent(this), this); 
        }

        public void SetLogicPositionUpdateFlag(bool value)
        {
            shouldUpdateToLogicalPositon = value;
        }

    }
}