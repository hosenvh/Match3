using Match3.Foundation.Base.EventManagement;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Game.Inbox;
using UnityEngine;


namespace Match3.Presentation.Inbox
{
    public class InboxTab : MonoBehaviour
    {
        [SerializeField] private Transform elementsContainer;
        [SerializeField] private GameObject emptyModeVisual;

        public void OpenInboxTab()
        {
            PropagateInboxTabOpenedEvent();

            void PropagateInboxTabOpenedEvent() => ServiceLocator.Find<EventManager>().Propagate(evt: new OnInboxTabOpened(this), sender: this);
        }

        public void GotoEmptyMode()
        {
            elementsContainer.RemoveChildren();
            emptyModeVisual.gameObject.SetActive(true);
        }

        public void ExitEmptyMode()
        {
            emptyModeVisual.gameObject.SetActive(false);
        }

        public InboxElement InstantiateIntoInbox(InboxElement element)
        {
            return Instantiate(element, parent: elementsContainer);
        }
    }

    public class OnInboxTabOpened : GameEvent
    {
        public InboxTab InboxTab { get; }

        public OnInboxTabOpened(InboxTab inboxTab)
        {
            InboxTab = inboxTab;
        }
    }
}