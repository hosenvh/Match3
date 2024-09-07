using Match3.Foundation.Base.ServiceLocating;
using Match3.Foundation.Base.Utility;
using UnityEngine;

namespace Match3.Overlay.Advertisement.Placements.Implementations
{
    public class MapEnteringAdPlacementController : MonoBehaviour
    {
        private void Start()
        {
            var adPlacementManager = ServiceLocator.Find<AdvertisementPlacementsManager>();

            if (adPlacementManager.IsAvailable<MapEnteringInterstitialAdPlacement>())
                adPlacementManager.Play<MapEnteringInterstitialAdPlacement>(new EmptyArgument(), onSuccess: delegate {  }, onFailure: delegate { });
        }
    }
}