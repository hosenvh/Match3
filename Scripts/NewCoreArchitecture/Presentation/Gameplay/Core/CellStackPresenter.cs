using Match3.Foundation.Base.EventManagement;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Game.Gameplay.Core;
using Match3.Presentation.Gameplay.PrefabDatabases;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Match3.Presentation.Gameplay.Core
{
    public struct CellStackPresenterClickedEvent : GameEvent
    {
        public readonly CellStackPresenter cellStackPresenter;
        public CellStackPresenterClickedEvent(CellStackPresenter cellStackPresenter)
        {
            this.cellStackPresenter = cellStackPresenter;
        }
    }

    public class CellStackPresenter : MonoBehaviour, IPointerDownHandler, Foundation.Base.ComponentSystem.Component
    {

        CellStack cellStack;
        Func<int, RectTransform> cellContainerGetter;

        public void Setup(CellStack cellStack, 
            Func<int, RectTransform> cellContainerGetter, 
            Func<CellAttachment, RectTransform> AttachmentContainerGetter)
        {
            this.cellStack = cellStack;
            this.cellContainerGetter = cellContainerGetter;
            cellStack.AddComponent(this);


            var cellArray = new List<Cell>(cellStack.Stack());
            cellArray.Reverse();

            for(int i = 0; i< cellArray.Count; ++i)
                SetupCellPresentationFor(cellArray[i], i);  
            
            foreach(var attachment in cellStack.Attachments())
                SetupCellAttachmentPresentationFor(attachment, AttachmentContainerGetter(attachment));
           
        }

        private void SetupCellAttachmentPresentationFor(CellAttachment attachment, RectTransform parent)
        {

            var presenter = Instantiate(ServiceLocator.Find<CellAttachmentPrefabDatabase>().PresenterPrefabFor(attachment), parent, false);
            presenter.transform.localPosition = this.transform.localPosition;
            presenter.Setup(attachment);
            //presenter.Setup(cell, this);
        }

        public void SetupCellPresentationFor(Cell cell, int order)
        {
            var parent = cellContainerGetter(order);
            var presenter = Instantiate(ServiceLocator.Find<CellPrefabDatabase>().PresenterPrefabFor(cell), parent, false);
            presenter.transform.localPosition = this.transform.localPosition;
            presenter.Setup(cell, this);
        }


        public void UpdateCellOrderings()
        {
            int order = cellStack.Stack().Count-1;
            foreach (var item in cellStack.Stack())
            {
                item.GetComponent<CellPresenter>().transform.SetParent(cellContainerGetter(order));
                order -= 1;
            }
        }

        public CellStack CellStack()
        {
            return cellStack;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
                ServiceLocator.Find<EventManager>().Propagate(new CellStackPresenterClickedEvent(this), this);
        }
    }
}