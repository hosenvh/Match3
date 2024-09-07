using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


namespace Match3.Main.VideoPlayer
{
    public class VideoProgressPresentation : MonoBehaviour,
                                             IDragHandler,
                                             IPointerDownHandler,
                                             IPointerUpHandler
    {
        [SerializeField] private RectTransform indicator;
        [SerializeField] private Image filler;

        private float totalWidth;
        private RectTransform rectTransform;
        private Action<float> onUpdateIndicatorPosition;
        private Action onPointerDown;
        private Action onPointerUp;

        private void Awake()
        {
            totalWidth = GetComponent<RectTransform>().rect.width;
            rectTransform = GetComponent<RectTransform>();
        }

        public void Setup(Action<float> onUpdateIndicatorPosition, Action onPointerDown, Action onPointerUp)
        {
            this.onUpdateIndicatorPosition = onUpdateIndicatorPosition;
            this.onPointerDown = onPointerDown;
            this.onPointerUp = onPointerUp;
        }

        public void UpdateProgress(float progress)
        {
            indicator.anchoredPosition = Vector2.right * (progress * totalWidth);
            filler.fillAmount = progress;
        }

        public void OnDrag(PointerEventData eventData)
        {
            onUpdateIndicatorPosition.Invoke(CalculateProgress(eventData.position));
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            onUpdateIndicatorPosition.Invoke(CalculateProgress(eventData.position));
            onPointerDown.Invoke();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            onUpdateIndicatorPosition.Invoke(CalculateProgress(eventData.position));
            onPointerUp.Invoke();
        }

        private float CalculateProgress(Vector2 clickedPosition)
        {
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform,
                                                                    clickedPosition,
                                                                    null,
                                                                    out localPoint);
            return Mathf.Clamp01((localPoint.x + (totalWidth / 2)) / totalWidth);
        }
    }
}