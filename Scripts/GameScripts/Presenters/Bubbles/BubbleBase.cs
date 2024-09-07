using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Match3.Utility;
using UnityEngine;


namespace Match3.Presentation.Bubbles
{
    public abstract class BubbleBase : MonoBehaviour
    {
        [SerializeField] protected RectTransform mainContainer;
        [SerializeField] protected RectTransform bubbleMainPart;
        [SerializeField] private float sizeOffset;

        private const float DURATION_TIME = 0.3f;

        private TouchDetector TouchDetector { get; set; }
        private bool IsOpen { get; set; }

        public TBubble Open<TBubble>() where TBubble : BubbleBase
        {
            if (CanOpenBubble())
            {
                IsOpen = true;
                PlayOpenAnimation();
            }

            return this as TBubble;

            bool CanOpenBubble() => IsOpen == false && DoesAnyTweenRunningOnGameObject(mainContainer.gameObject) == false;
            void PlayOpenAnimation() => mainContainer.DOScale(1, DURATION_TIME).SetEase(Ease.OutBack);
        }

        public virtual BubbleBase SetAutoSize()
        {
            List<RectTransform> childs = GetChilds();
            SetSize(GetMaxWidth() * sizeOffset, GetMaxHeight() * sizeOffset);

            return this;

            float GetMaxWidth()
            {
                return childs.Select(child => child.rect.width).Max();
            }

            float GetMaxHeight()
            {
                return childs.Select(child => child.rect.height).Max();
            }
        }

        private List<RectTransform> GetChilds()
        {
            List<RectTransform> childs = new List<RectTransform>();
            for (int i = 0; i < bubbleMainPart.childCount; i++)
                childs.Add(bubbleMainPart.GetChild(i).GetComponent<RectTransform>());

            return childs;
        }

        public BubbleBase SetSize(float width, float height)
        {
            bubbleMainPart.sizeDelta = new Vector2(width, height);
            return this;
        }

        public BubbleBase SetCloseOnOutsideClick()
        {
            if (TouchDetector == null)
            {
                TouchDetector = gameObject.AddComponent<TouchDetector>();
                TouchDetector.Setup(onUserTouch: () => Close());
            }

            return this;
        }

        public BubbleBase Close()
        {
            if (CanCloseBubble())
                PlayCloseAnimation();

            return this;

            bool CanCloseBubble() => IsOpen && DoesAnyTweenRunningOnGameObject(mainContainer.gameObject) == false;

            void PlayCloseAnimation() => mainContainer.DOScale(0, DURATION_TIME).SetEase(Ease.InBack).OnComplete(() => { IsOpen = false; });
        }

        public BubbleBase SetPosition(Vector3 position)
        {
            mainContainer.position = position;
            return this;
        }

        public BubbleBase SetParent(Transform parent)
        {
            transform.SetParent(parent);
            return this;
        }

        private bool DoesAnyTweenRunningOnGameObject(GameObject gameObject)
        {
            return DOTween.IsTweening(gameObject);
        }
    }
}