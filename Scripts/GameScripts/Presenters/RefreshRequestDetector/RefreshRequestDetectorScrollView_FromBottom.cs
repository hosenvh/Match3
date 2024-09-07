using Medrick.Base.Utility.Extensions;
using UnityEngine;


namespace Match3.Presentation.RefreshRequestDetector
{
    public class RefreshRequestDetectorScrollView_FromBottom : RefreshRequestDetectorScrollView
    {
        [SerializeField] private float scrollRectContentMinPosToStartRefreshRequest = 70;
        [SerializeField] private float scrollRectContentMaxPosToStartRefreshRequest = 20;

        protected override float GetNormalizedDragAmount()
        {
            var scrollPositionY = scrollRect.content.anchoredPosition.y;
            return scrollPositionY.Normalize(currentLowRange: scrollRectContentMinPosToStartRefreshRequest, currentHighRange: scrollRectContentMaxPosToStartRefreshRequest);
        }
    }
}