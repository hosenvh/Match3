using System;
using Match3.Foundation.Base.EventManagement;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.Input;
using Match3.Game.Gameplay.SubSystems.PowerUpManagement;
using Match3.Game.Gameplay.Swapping;
using Match3.Presentation.Gameplay.Core;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Match3.Presentation.Gameplay
{
    public class BoardPresentationInputController : MonoBehaviour , EventListener
    {
        public BoardPresenterNew boardPresenter;
        public float dragThreshold;
        public float swapDuration;
        public RectTransform rootTileStackTransform;

        public Transform selectionFrame;

        TileStackPresenter selectedTileStackPresenter = null;

        InputSystem inputSystem;

        void Start()
        {
            ServiceLocator.Find<EventManager>().Register(this);
            selectionFrame.gameObject.SetActive(false);

            inputSystem = boardPresenter.GetGameplaySystem<InputSystem>();
        }

        void OnDestroy()
        {
            ServiceLocator.Find<EventManager>().UnRegister(this);
        }

        private void HandleTileSelection(TileStackPresenter tileStackPresenter)
        {
            if (selectedTileStackPresenter == null)
            {
                TrySetSelectedTileStack(tileStackPresenter);
            }
            else if (selectedTileStackPresenter != tileStackPresenter)
            {
                if (IsAjacentWithCurrentSelected(tileStackPresenter))
                    StartTileSwap(selectedTileStackPresenter, DirectionFromCurrentSelected(tileStackPresenter));
                else
                    TrySetSelectedTileStack(tileStackPresenter);
            }
            else
                ActivateTileStack(selectedTileStackPresenter);
        }

        private void HandleCellSelectionSelection(CellStackPresenter cellStackPresenter)
        {
            if (selectedTileStackPresenter == null || cellStackPresenter.CellStack().HasTileStack())
                return;

            if (IsAjacentWithCurrentSelected(cellStackPresenter))
                StartTileSwap(selectedTileStackPresenter, DirectionFromCurrentSelected(cellStackPresenter));
        }

        private void ActivateTileStack(TileStackPresenter tilestackPresenter)
        {
            var command = new ActivteCommandInput(tilestackPresenter.tileStack);

            inputSystem.Apply(command);

            ClearSelectedTileStack();
        }

        private Direction DirectionFromCurrentSelected(TileStackPresenter tileStackPresenter)
        {
            return boardPresenter.GameplayController().GameBoard().RelativeDirectionOf(selectedTileStackPresenter.tileStack, tileStackPresenter.tileStack);
        }

        private Direction DirectionFromCurrentSelected(CellStackPresenter cellStackPresenter)
        {
            return boardPresenter.GameplayController().GameBoard().CellStackBoard().RelativeDirectionOf(
                selectedTileStackPresenter.tileStack.Parent().Position(), 
                cellStackPresenter.CellStack().Position());
        }

        private bool IsAjacentWithCurrentSelected(TileStackPresenter tileStackPresenter)
        {
            return boardPresenter.GameplayController().GameBoard().AreAdjacent(selectedTileStackPresenter.tileStack, tileStackPresenter.tileStack);
        }

        private bool IsAjacentWithCurrentSelected(CellStackPresenter cellStackPresenter)
        {
            return boardPresenter.GameplayController().GameBoard().CellStackBoard().AreAdjacent(
                selectedTileStackPresenter.tileStack.Parent().Position(), 
                cellStackPresenter.CellStack().Position());
        }


        private void HandleTileDraging(TileStackPresenter tileStackPresenter, PointerEventData eventData)
        {
            if (tileStackPresenter != selectedTileStackPresenter)
                return;
            var direction = DragDirectionFor(eventData);

            if (direction != Direction.None)
                StartTileSwap(tileStackPresenter, direction);
        }

        public Direction DragDirectionFor(PointerEventData eventData)
        {
            Vector2 localPressPos;
            Vector2 localCurrentPos;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(rootTileStackTransform, eventData.pressPosition, eventData.pressEventCamera, out localPressPos);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rootTileStackTransform, eventData.position, eventData.pressEventCamera, out localCurrentPos);

            var xOffset = localPressPos.x - localCurrentPos.x;
            var yOffset = localPressPos.y - localCurrentPos.y;

            var direction = Direction.None;

            if (xOffset >= dragThreshold)
                direction = Direction.Left;
            else if (xOffset <= -dragThreshold)
                direction = Direction.Right;
            else if (yOffset >= dragThreshold)
                direction = Direction.Down;
            else if (yOffset <= -dragThreshold)
                direction = Direction.Up;

            return direction;
        }

        public void StartPower()
        {
            if(selectedTileStackPresenter != null)
            {
                var command = new HammerPowerUpInputCommand(selectedTileStackPresenter.tileStack.Parent());
                inputSystem.Apply(command);

                ClearSelectedTileStack();
            }
        }


        void StartTileSwap(TileStackPresenter tileStackPresenter, Direction direction)
        {
            var command = new SwapCommandInput(tileStackPresenter.tileStack, direction);
            inputSystem.Apply(command);
            ClearSelectedTileStack();
        }

        void ClearSelectedTileStack()
        {
            selectedTileStackPresenter = null;
            selectionFrame.gameObject.SetActive(false);
        }


        void TrySetSelectedTileStack(TileStackPresenter presenter)
        {
            if (IsNotSwappable(presenter.tileStack))
                return;
            selectionFrame.localPosition = presenter.tileStack.Parent().GetComponent<CellStackPresenter>().transform.localPosition;
            selectedTileStackPresenter = presenter;
            selectionFrame.gameObject.SetActive(true);
        }

        bool IsNotSwappable(TileStack tileStack)
        {
            return tileStack.IsDepleted()
                || tileStack.Top().GetComponent<TileUserInteractionProperties>().isSwappable == false;
        }

        public void OnEvent(GameEvent evt, object sender)
        {
            switch (evt)
            {
                case TileStackPresenterClickedEvent clickEvent:
                    HandleTileSelection(clickEvent.tileStackPresenter);
                    break;
                case CellStackPresenterClickedEvent clickEvent:
                    HandleCellSelectionSelection(clickEvent.cellStackPresenter);
                    break;
                case TileStackPresenterDraggedEvent dragEvent:
                    HandleTileDraging(dragEvent.tileStackPresenter, dragEvent.eventData);
                    break;
                    
            }
        }

    }
}