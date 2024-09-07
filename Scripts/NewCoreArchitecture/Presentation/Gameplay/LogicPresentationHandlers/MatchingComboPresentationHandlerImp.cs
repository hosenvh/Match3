using Match3.Foundation.Base.ServiceLocating;
using Match3.Game.Gameplay.MatchingCombo;
using UnityEngine;

namespace Match3.Presentation.Gameplay.LogicPresentationHandlers
{
    public class MatchingComboPresentationHandlerImp : MonoBehaviour, MatchingComboPresentationHandler
    {

        GameplaySoundManager gameplaySoundManager;

        void Awake()
        {
            gameplaySoundManager = ServiceLocator.Find<GameplaySoundManager>();
        }

        public void HandleCombo(int comboCount)
        {
            gameplaySoundManager.PlayMatchSound(comboCount);
        }
    }
}