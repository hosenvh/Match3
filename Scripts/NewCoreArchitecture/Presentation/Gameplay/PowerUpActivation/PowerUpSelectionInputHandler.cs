using Match3.Foundation.Base.EventManagement;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Game.Gameplay;
using Match3.Game.Gameplay.Cells;
using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.Input;
using Match3.Presentation.Gameplay.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Match3.Presentation.Gameplay.PowerUpActivation
{
    public class PowerUpSelectedEvent : GameEvent
    {

    }
    //TODO: Rename This.
    public class PowerUpSelectionInputHandler : MonoBehaviour, IPointerDownHandler, IDragHandler
    {
        public Image deactiveTileImagePrefab;
        public BoardPresentationInputController boardPresentationInputController;
        public GameObject selectionFrame;


        public RectTransform insideImageContainer;

        public RectTransform leftOutsideImage;
        public RectTransform rightOutsideImage;
        public RectTransform topOutsideImage;
        public RectTransform bottomOutsideImage;


        PowerUpActivator currentPowerUpActivator;


        public void Setup(GameplayState gameState)
        {
            this.gameObject.SetActive(false);
            var dimensions = gameState.root.boardPresenter.BoardDimensions();
            var gameBoard = gameState.gameplayController.GameBoard();
       
            foreach (var element in gameBoard.DefaultCellBoardIterator())
            {
                if(element.value.Top() is EmptyCell)
                {
                    var image = Instantiate(deactiveTileImagePrefab, insideImageContainer, false);
                    image.transform.position = dimensions.LogicalPosToPresentaionalPos(element.value.Position());
                }
            }

            Vector2 position;
            Vector2 sizeDelta;


            position = leftOutsideImage.position;
            position.x = dimensions.origin.x - dimensions.tileSize.x/2;
            leftOutsideImage.anchoredPosition = position;

            position = rightOutsideImage.position;
            position.x = dimensions.origin.x + dimensions.boardSize.x - dimensions.tileSize.x / 2;
            rightOutsideImage.anchoredPosition = position;


            position = topOutsideImage.position;
            sizeDelta = topOutsideImage.sizeDelta;
            position.y = dimensions.origin.y + dimensions.tileSize.y / 2;
            sizeDelta.x = dimensions.boardSize.x;
            topOutsideImage.position = position;
            topOutsideImage.sizeDelta = sizeDelta;



            position = bottomOutsideImage.position;
            sizeDelta = bottomOutsideImage.sizeDelta;
            position.y = dimensions.origin.y - dimensions.boardSize.y + dimensions.tileSize.y / 2;
            sizeDelta.x = dimensions.boardSize.x;
            bottomOutsideImage.position = position;
            bottomOutsideImage.sizeDelta = sizeDelta;
        }

        public void OnPointerDown(PointerEventData eventData)
        {


            CellStackPresenter cellStackPresenter = FindCellStackIn(eventData);


            if (cellStackPresenter != null && cellStackPresenter.CellStack().Top() is EmptyCell == false)
            {
                if (currentPowerUpActivator.AcceptsSelection(cellStackPresenter))
                {
                    selectionFrame.SetActive(true);
                    selectionFrame.transform.position = cellStackPresenter.transform.position;

                    currentPowerUpActivator.OnCellStackSelected(cellStackPresenter);
                    //onCellStackSelectionAction(cellStackPresenter);
                }
            }
            else
                Disable();

        }

        CellStackPresenter FindCellStackIn(PointerEventData eventData)
        {
            List<RaycastResult> hitResults = new List<RaycastResult>();


            EventSystem.current.RaycastAll(eventData, hitResults);


            foreach (var hit in hitResults)
            {
                if (hit.gameObject.GetComponent<CellStackPresenter>() != null)
                   return hit.gameObject.GetComponent<CellStackPresenter>();
                
            }

            return null;
        }


        public void OnDrag(PointerEventData eventData)
        {
            if (currentPowerUpActivator is HandPowerUpActivator == false)
                return;

            PointerEventData newData = new PointerEventData(EventSystem.current)
            {
                pointerId = -1,
            };

            newData.position = eventData.pressPosition;
            var cellStack = FindCellStackIn(newData);
            if (cellStack == null || currentPowerUpActivator.AcceptsSelection(cellStack) == false)
                return;

            var direction = boardPresentationInputController.DragDirectionFor(eventData);

            if (direction == Direction.None)
                return;

            currentPowerUpActivator.As<HandPowerUpActivator>().OnCellStackSelectedByDrag(cellStack, direction);


        }

        public void EnablePowerUpSelection(PowerUpActivator powerUpActivator)
        {
            selectionFrame.SetActive(false);
            this.gameObject.SetActive(true);
            this.currentPowerUpActivator = powerUpActivator;
            ServiceLocator.Find<EventManager>().Propagate(new PowerUpSelectedEvent(), this);
        }



        public void Disable()
        {
            this.currentPowerUpActivator.OnSelectionCanceled();
            selectionFrame.SetActive(false);
            this.gameObject.SetActive(false);
            currentPowerUpActivator = null;
        }

    }
}