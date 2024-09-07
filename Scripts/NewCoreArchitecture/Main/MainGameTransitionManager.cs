using System;
using Match3.Game;
using Match3.Game.NeighborhoodChallenge;
using Match3.Foundation.Base.EventManagement;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Presentation.Gameplay;
using Match3.Presentation.NeighborhoodChallenge;
using Match3.Presentation.TransitionEffects;


namespace Match3.Main
{
    public class OnMainMenuTransitionEffectCompletedEvent : GameEvent
    {
       
    }

    public class MainGameTransitionManager : GameTransitionManager
    {
        public void GoToLastMap<TTransitionEffect>() where TTransitionEffect : Presentation.TransitionEffects.TransitionEffect
        {
            GoToLastMap<TTransitionEffect>(keepPopus: false, onMapLoadComplete: () => {});
        }

        public void GoToLastMap<TTransitionEffect>(bool keepPopus, Action onMapLoadComplete) where TTransitionEffect : Presentation.TransitionEffects.TransitionEffect
        {
            GoToMap<TTransitionEffect>(GameManager().mapManager.GetLastLoadedMapId(), keepPopus, onMapLoadComplete);
        }

        public void GoToMap<TTransitionEffect>(string mapId, bool keepPopups, Action onMapLoadComplete = null)
            where TTransitionEffect : Presentation.TransitionEffects.TransitionEffect
        {
            if(!keepPopups)
                GameManager().ClosePopup(true);
            
            var transitionEffect = Activator.CreateInstance<TTransitionEffect>();
            transitionEffect.StartTransitionEffect(
                onFadeInFinished: () => OpenMapAndSetup(mapId, keepPopups, onMapLoadComplete),
                onFadeOutFinished: () => ServiceLocator.Find<EventManager>().Propagate(
                    evt: new OnMainMenuTransitionEffectCompletedEvent(), 
                    sender: this
                )
            );
        }

        private void OpenMapAndSetup(string mapId, bool isContinue, Action onMapLoadComplete = null)
        {
            var map = GameManager().mapManager.OpenMap(mapId);
            map.Setup(() =>
            {
                if (!isContinue)
                {
                    GameManager().musicManager.StartSceneMusic(0, 2f);
                    OpenMainMenu();
                }
                    
                onMapLoadComplete?.Invoke();
            });
        }

        private void OpenMainMenu()
        {
            var mainMenu = GameManager().OpenPopup<Popup_MainMenu>();
            mainMenu.Setup();
        }
        
        
        
        public void GoToNCLevel(BoardConfig level, int index)
        {
            var darkInTransitionEffect = new DarkInTransitionEffect();
            darkInTransitionEffect.StartTransitionEffect(() =>
            {
                Base.gameManager.ClosePopup(true);
                Base.gameManager.GoToState<NeighborhoodChallengeGameplayState>().Setup(level, index);
            }, delegate {  });
        }

        public void GoToNCLobby(NCInsideLobbyController controller)
        {
            var darkInTransitionEffect = new DarkInTransitionEffect();
            darkInTransitionEffect.StartTransitionEffect(() =>
            {
                Base.gameManager.ClosePopup(true);
                Base.gameManager.GoToState<State_NeighborhoodChallengeLobby>().Setup(controller);
            }, delegate {  });
        }

        public void GoToLevel(BoardConfig boardConfig, int selectedLevelIndex)
        {
            var darkInTransitionEffect = new DarkInTransitionEffect();
            darkInTransitionEffect.StartTransitionEffect(() =>
            {
                Base.gameManager.ClosePopup(true);
                Base.gameManager.GoToState<CampaignGameplayState>().Setup(boardConfig, selectedLevelIndex);
            }, delegate {  });
        }
        
        public bool IsInCampaign()
        {
            return Base.gameManager.CurrentState is CampaignGameplayState;
        }
        
        public bool IsInMap()
        {
            return GameManager().mapManager.IsInMap();
        }

        global::Game GameManager()
        {
            return global::Game.gameManager;
        }
    }
}