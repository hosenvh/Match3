using Match3.Foundation.Base.EventManagement;
using UnityEngine.EventSystems;

namespace Match3.Presentation.Gameplay.Core
{
    public class TileStackPresenterEvent : GameEvent
    {
        public readonly TileStackPresenter tileStackPresenter;

        public TileStackPresenterEvent(TileStackPresenter tileStackPresenter)
        {
            this.tileStackPresenter = tileStackPresenter;
        }
    }

    public class TileStackPresenterClickedEvent : TileStackPresenterEvent
    {

        public TileStackPresenterClickedEvent(TileStackPresenter tileStackPresenter)
            : base(tileStackPresenter)
        {
        }
    }

    public class TileStackPresenterRightClickedEvent : TileStackPresenterEvent
    {
        public TileStackPresenterRightClickedEvent(TileStackPresenter tileStackPresenter) : base(tileStackPresenter)
        {
        }
    }

    public class TileStackPresenterDraggedEvent : TileStackPresenterEvent
    {

        public readonly PointerEventData eventData;

        public TileStackPresenterDraggedEvent(
            TileStackPresenter tileStackPresenter, PointerEventData eventData)
            : base(tileStackPresenter)
        {
            this.eventData = eventData;
        }
    }

}