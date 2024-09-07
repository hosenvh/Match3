
using Match3.Foundation.Base.Configuration;
using Match3.Foundation.Base.EventManagement;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Foundation.Unity.TimeManagement;
using System;
using System.Collections.Generic;

namespace Match3.Game.NeighborhoodChallenge
{
    public class NeighborhoodChallengeTutorialManager : Service, EventListener
    {
        List<int> tutorialSequence = new List<int>();

        TutorialManager tutorialManager;
        float stepDelay;
        float startDelay;
        Popup_TouchDisabler touchDisabler;

        public NeighborhoodChallengeTutorialManager(TutorialManager tutorialManager)
        {
            this.tutorialManager = tutorialManager;

            ServiceLocator.Find<ConfigurationManager>().Configure(this);
            ServiceLocator.Find<EventManager>().Register(this);
        }


        public List<int> GetTutorialSequence()
        {
            return tutorialSequence;
        }
        

        public void SetDelays(float startDelay, float stepDelay)
        {
            this.startDelay = startDelay;
            this.stepDelay = stepDelay;
        }

        public void SetTutorialSequence(List<int> tutorialSequence)
        {
            this.tutorialSequence = tutorialSequence;
        }

        public void Replay()
        {
            foreach (var id in tutorialSequence)
                tutorialManager.SetTutorialState(id, 0);

            touchDisabler = Base.gameManager.OpenPopup<Popup_TouchDisabler>();
            StartTutorial();
        }

        void StartTutorial()
        {
            StartTutorialStep(0);        
        }

        void StartTutorialStep(int step)
        {
            if (step >= tutorialSequence.Count)
            {
                touchDisabler.Close();
                touchDisabler = null;
                return;
            }

            tutorialManager.CheckThenShowTutorial(tutorialSequence[step], stepDelay, onHide: () => StartTutorialStep(step + 1));
        }
        public void OnEvent(GameEvent evt, object sender)
        {
            if(evt is NeighborhoodChallengeLobbyOpenedEvent)
            {
                ServiceLocator.Find<EventManager>().UnRegister(this);
                TryStartTutorialForLobbyEntering();
            }

        }

        private void TryStartTutorialForLobbyEntering()
        {
            if (tutorialManager.IsTutorialShowed(tutorialSequence[0]))
                return;
            touchDisabler = Base.gameManager.OpenPopup<Popup_TouchDisabler>();

            ServiceLocator.Find<UnityTimeScheduler>().Schedule(startDelay, StartTutorial, this);

        }
    }
}