using SeganX;
using UnityEngine;


namespace Match3.Presentation
{
    public class ProgressBarBubble : MonoBehaviour
    {
        [SerializeField] private RectTransform bubble;
        [SerializeField] private LocalText text;

        private RectTransform rectTransform;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
        }

        public void Setup(string text, float progressAmount)
        {
            Vector2 position = bubble.anchoredPosition;
            position.x = rectTransform.rect.width * progressAmount;
            bubble.anchoredPosition = position;

            this.text.SetText(text);
        }
    }
}