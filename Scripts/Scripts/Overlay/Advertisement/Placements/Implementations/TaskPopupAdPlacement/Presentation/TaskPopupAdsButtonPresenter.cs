using Match3.Foundation.Base.ServiceLocating;
using Match3.Overlay.Advertisement.Placements;
using Match3.Overlay.Advertisement.Placements.Implementations.TaskPopupAdPlacement;
using Match3.Overlay.Advertisement.Placements.Implementations.TimeBasedAdPlacementBase.Presentation;
using Match3.Presentation.HUD;
using UnityEngine;
using UnityEngine.UI;


namespace Match3
{
    public class TaskPopupAdsButtonPresenter : MonoBehaviour
    {
        [SerializeField] private Button unityButton;

        private TimeBasedAdPlacementButtonClickHandler<TaskPopupAdPlacement> clickHandler;

        private void Awake()
        {
            if (ShouldGetHidden())
                GetDestroyed();

            bool ShouldGetHidden()
            {
                return IsPlacementAvailable() == false;
                bool IsPlacementAvailable() => ServiceLocator.Find<AdvertisementPlacementsManager>().Find<TaskPopupAdPlacement>().IsAvailable();
            }
        }

        public void Setup(HudPresentationController hudPresentationController)
        {
            Initialize();

            void Initialize()
            {
                clickHandler = new TimeBasedAdPlacementButtonClickHandler<TaskPopupAdPlacement>(unityButton, hudPresentationController, onAdsShownSuccessfully: GetDestroyed);
            }
        }

        private void GetDestroyed()
        {
            Destroy(gameObject);
        }
    }
}