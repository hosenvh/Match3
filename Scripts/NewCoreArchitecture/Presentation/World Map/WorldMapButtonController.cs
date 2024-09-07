using System;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Foundation.Base.TaskManagement;
using UnityEngine;
using UnityEngine.UI;


namespace Match3.Presentation.WorldMap
{
    public class WorldMapButtonController : MonoBehaviour
    {
        public Button button;
        public int tutorialIndex;

        private void Awake()
        {
            button.onClick.AddListener(OpenWorldMap);

            if(ShouldRegisterTutorialMainMenuTask())
                ServiceLocator.Find<MainMenuTaskChainService>().AddTask(new WorldMapTutorialMainMenuTask(tutorialIndex), priority: MainMenuTasksPriorities.WorldMapTutorialTask, "WorldMapTutorialTask");
        }

        private void OnEnable()
        {
            button.gameObject.SetActive(IsMoreThanOneMapUnlocked());
        }

        private bool IsMoreThanOneMapUnlocked()
        {
            var mapManager = Base.gameManager.mapManager;
            var mapsMetadata = mapManager.MapsMetaDatabase.MapsMetadata;
            var unlockedMapsCount = 0;
            
            foreach (var mapMetaData in mapsMetadata)
            {
                if (mapManager.IsMapUnlocked(mapMetaData.mapId))
                    unlockedMapsCount++;
            }

            return unlockedMapsCount > 1;
        }

        private void OpenWorldMap()
        {
            Base.gameManager.tutorialManager.CheckAndHideTutorial(tutorialIndex);
            Base.gameManager.OpenPopup<Popup_WorldMap>().Setup(Base.gameManager.mapManager);
        }


        private bool ShouldRegisterTutorialMainMenuTask()
        {
            return IsSafeToRunWorldMapTutorial_TEMPORARY_METHOD() && !Base.gameManager.tutorialManager.IsTutorialShowed(tutorialIndex) && IsMoreThanOneMapUnlocked();

            bool IsSafeToRunWorldMapTutorial_TEMPORARY_METHOD() => WillWorldMapTutorialHighlightCorrectLocation();
            bool WillWorldMapTutorialHighlightCorrectLocation() => IsClanButtonAvailable();
            bool IsClanButtonAvailable() => ServiceLocator.Find<ServerConfigManager>().data.config.clanServerConfig.IsClanActive; // TODO: Note that the fact that WorldMapTutorial should only play when the clan is activated,
                                                                                                                                  // is because our clan button is positioned on the left side of the world map button. Additionally,
                                                                                                                                  // there may be a temporary period when the clan is not activated for some users to test our backend endurance and other factors.
                                                                                                                                  // However, it is expected that the clan will eventually be activated for all users, and this field should be removed,
                                                                                                                                  // ensuring that the WorldMapTutorial can be safely displayed at that point.
        }
    }    
}


