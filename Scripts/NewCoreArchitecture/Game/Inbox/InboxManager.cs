using System.Collections.Generic;
using System.Linq;
using Match3.Foundation.Base.EventManagement;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Presentation.Inbox;
using Match3.Presentation.MainMenu;
using Medrick.Base.Utility.Extensions;
using UnityEngine;


namespace Match3.Game.Inbox
{
    public class InboxManager : EventListener, Service
    {
        private class RegisteredInboxElement
        {
            public int OrderIndex { get; }
            public InboxElement Element { get; }
            public InboxElementData Data { get; }

            public RegisteredInboxElement(int orderIndex, InboxElement element, InboxElementData data)
            {
                OrderIndex = orderIndex;
                Element = element;
                Data = data;
            }
        }

        private readonly Dictionary<string, RegisteredInboxElement> registeredElements = new Dictionary<string, RegisteredInboxElement>();

        public InboxManager()
        {
            RegisterSelfAsEventListener();

            void RegisterSelfAsEventListener() => ServiceLocator.Find<EventManager>().Register(this);
        }

        public void RegisterInboxElement<TElementData>(string id, int orderIndex, InboxElement<TElementData> inboxElement, TElementData elementSetupData) where TElementData : InboxElementData
        {
            registeredElements.Add(id, new RegisteredInboxElement(orderIndex, inboxElement, elementSetupData));
        }

        public void UnRegisterInboxElement(string id)
        {
            registeredElements.Remove(key: id);
        }

        public void UnRegisterInboxElementByMatchingWildcardPattern(string idMatchingPattern)
        {
            registeredElements.RemoveElementsByMatchingKeys(idMatchingPattern);
        }

        public void OnEvent(GameEvent evt, object sender)
        {
            if (evt is OnInboxTabOpened inboxTabEvent)
                UpdateInboxTabPresenter(inboxTabEvent.InboxTab);
        }

        private void UpdateInboxTabPresenter(InboxTab inboxTab)
        {
            inboxTab.GotoEmptyMode();
            if (IsAnyThingToShow())
            {
                inboxTab.ExitEmptyMode();
                InstantiateToInboxElements();
            }

            bool IsAnyThingToShow() => registeredElements.Count != 0;

            void InstantiateToInboxElements()
            {
                var registeredElementsList = registeredElements.Values.ToList().OrderBy(element => element.OrderIndex);
                foreach (RegisteredInboxElement registeredElement in registeredElementsList)
                {
                    InboxElement instantiatedElement = inboxTab.InstantiateIntoInbox(registeredElement.Element);
                    instantiatedElement.Setup(registeredElement.Data);
                }
            }
        }

        public void TryForceUpdateInboxTabPresenter()
        {
            InboxTab inboxTab = Object.FindObjectOfType<InboxTab>();
            bool isInboxTabPresenterOpen = inboxTab != null;
            if (isInboxTabPresenterOpen)
                UpdateInboxTabPresenter(inboxTab);
        }

        public void ClearAllRegisteredInboxElements_Debug()
        {
            registeredElements.Clear();
        }

        public void OpenInboxTab()
        {
            new TaskPopupOpener().TryOpenTaskPopup().OpenInboxTab();
        }
    }
}