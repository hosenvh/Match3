using UnityEngine;


namespace Match3.Game.Inbox
{
    public interface InboxElementData
    {
    }

    public abstract class InboxElement : MonoBehaviour
    {
        public abstract void Setup(InboxElementData data);
    }

    public abstract class InboxElement<TData> : InboxElement where TData : InboxElementData
    {
        protected TData data;

        public override void Setup(InboxElementData data)
        {
            this.data = (TData) data;
            DoInternalSetups(this.data);
        }

        protected abstract void DoInternalSetups(TData data);
    }
}