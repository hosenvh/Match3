using Medrick.Base.Utility.Extensions;
using UnityEngine;


namespace Match3.Presentation.RefreshRequestDetector
{
    public class RefreshRequestDetectorScrollView_FromTop : RefreshRequestDetectorScrollView
    {
        [SerializeField] private float scrollRectContentMinPosToStartRefreshRequest = -70;
        [SerializeField] private float scrollRectContentMaxPosToStartRefreshRequest = -20;

        protected override float GetNormalizedDragAmount()
        {
            var scrollPositionY = scrollRect.content.anchoredPosition.y;
            return 1 - scrollPositionY.Normalize(currentLowRange: scrollRectContentMinPosToStartRefreshRequest, currentHighRange: scrollRectContentMaxPosToStartRefreshRequest);
        }
    }
}