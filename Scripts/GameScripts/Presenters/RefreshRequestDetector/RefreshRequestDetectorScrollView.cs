using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


namespace Match3.Presentation.RefreshRequestDetector
{
    public abstract class RefreshRequestDetectorScrollView : MonoBehaviour, IDragHandler, IEndDragHandler
    {
        private const float REFRESH_REQUEST_NEEDED_DRAG_AMOUNT = 0.9f;

        [SerializeField] protected ScrollRect scrollRect;
        [SerializeField] private CanvasGroup refreshIndicator;

        private Action onRefreshRequested;

        public void Setup(Action onRefreshRequested)
        {
            this.onRefreshRequested = onRefreshRequested;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (GetNormalizedDragAmount() > REFRESH_REQUEST_NEEDED_DRAG_AMOUNT)
                onRefreshRequested.Invoke();
        }

        protected abstract float GetNormalizedDragAmount();

        public void OnDrag(PointerEventData eventData)
        {
            refreshIndicator.alpha = GetNormalizedDragAmount();
        }
    }
}