using Match3.Foundation.Base.ServiceLocating;
using Match3.Foundation.Unity.TimeManagement;
using Match3.Game;
using Match3.Game.Map;
using Match3.Presentation.TransitionEffects;
using SeganX;
using UnityEngine;


namespace Match3.Presentation.WorldMap
{
    public class Popup_WorldMap : GameState
    {
        
        [SerializeField] private WorldMapPlace[] mapPlaces;
        [SerializeField] private CurrentMapPlacePointer mapPlacePointer;

        [Space(10)]
        [SerializeField] private AnimationClip mapPlaceStartAnimationClip;
        [SerializeField] private AnimationClip mapPlaceAppearAnimationClip;
        [SerializeField] private AnimationClip mapPlaceDisappearAnimationClip;

        private bool isMapChanging = false;
        private WorldMapPlace currentPlace;
        
        
        
        public void Setup(MapManager mapManager)
        {
            currentPlace = TryFindWorldMapPlace(mapManager.CurrentMapId);
            InitializeWorldMapPlaces();
            mapPlacePointer.Setup(currentPlace);
        }

        private void InitializeWorldMapPlaces()
        {
            var scheduler = ServiceLocator.Find<UnityTimeScheduler>();
            foreach (var mapPlace in mapPlaces)
            {
                mapPlace.SetOnButtonClickListener(OnWorldMapPlaceClicked);
                if (mapPlace == currentPlace)
                {
                    mapPlace.gameObject.SetActive(false);
                }
                else
                {
                    scheduler.Schedule(0.5f, () =>
                    {
                        mapPlace.PlayAnimation(mapPlaceStartAnimationClip, delegate {  });
                    }, this);
                }
            }
        }

        private WorldMapPlace TryFindWorldMapPlace(string mapId)
        {
            foreach (var mapPlace in mapPlaces)
            {
                if (mapPlace.PlaceMapId == mapId)
                    return mapPlace;
            }

            return null;
        }

        private void OnWorldMapPlaceClicked(WorldMapPlace clickedPlace)
        {
            if (clickedPlace != currentPlace && !isMapChanging)
            {
                isMapChanging = true;
                mapPlacePointer.MoveToPlace(clickedPlace, () =>
                {
                    ServiceLocator.Find<GameTransitionManager>().GoToMap<CloudTransitionEffect>(clickedPlace.PlaceMapId, false);
                });
                currentPlace.gameObject.SetActive(true);
                currentPlace.PlayAnimation(mapPlaceAppearAnimationClip, delegate {  });
                clickedPlace.PlayAnimation(mapPlaceDisappearAnimationClip, () =>
                {
                    clickedPlace.gameObject.SetActive(false);
                });
                currentPlace = clickedPlace;
            }
        }

        public override void Back()
        {
            ServiceLocator.Find<UnityTimeScheduler>().UnSchedule(this);
            base.Back();
        }
        
    }    
}


