using System;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Game.NeighborhoodChallenge;


namespace Match3.Game
{
    public interface GameTransitionManager : Service
    {
        void GoToLastMap<TTransitionEffect>() where TTransitionEffect : Presentation.TransitionEffects.TransitionEffect;
        void GoToLastMap<TTransitionEffect>(bool keepPopups, Action onMapLoadComplete) where TTransitionEffect : Presentation.TransitionEffects.TransitionEffect;

        void GoToMap<TTransitionEffect>(string mapId, bool keepPopups, Action onMapLoadComplete = null)
            where TTransitionEffect : Presentation.TransitionEffects.TransitionEffect;
        
        void GoToNCLobby(NCInsideLobbyController controller);

        void GoToNCLevel(BoardConfig level, int index);

        void GoToLevel(BoardConfig boardConfig, int selectedLevelIndex);

        bool IsInCampaign();
        
        bool IsInMap();
        
        
    }
}