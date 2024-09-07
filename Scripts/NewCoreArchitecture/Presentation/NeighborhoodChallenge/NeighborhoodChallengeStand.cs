using Match3.Foundation.Base.EventManagement;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Game.NeighborhoodChallenge;
using UnityEngine;

namespace Match3.Presentation.NeighborhoodChallenge
{
    public class NeighborhoodChallengeStand : MonoBehaviour, EventListener
    {
        private void Start()
        {
            var isActive = ServiceLocator.Find<NeighborhoodChallengeManager>().IsUnlocked();
            this.gameObject.SetActive(isActive);

            if(!isActive)
                ServiceLocator.Find<EventManager>().Register(this);
        }

        private void OnDestroy()
        {
            ServiceLocator.Find<EventManager>().UnRegister(this);
        }

        public void EnterChallenge()
        {
            ServiceLocator.Find<NeighborhoodChallengeManager>().GetController<NCLobbyEnteringController>().EnterLobby();
        }

        public void OnEvent(GameEvent evt, object sender)
        {
            if (evt is NeighborhoodChallengeUnlockedEvent)
            {
                this.gameObject.SetActive(true);
                ServiceLocator.Find<EventManager>().UnRegister(this);
            }
        }
    }
}