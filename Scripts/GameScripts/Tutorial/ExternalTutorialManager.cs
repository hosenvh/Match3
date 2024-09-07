using System.Collections.Generic;
using Match3.Foundation.Base.Configuration;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Foundation.Unity.TimeManagement;



namespace Match3.Game.Tutorial
{
    public abstract class ExternalTutorialManager
    {
        private List<int> tutorialSequence = new List<int>();

        private float stepDelay;
        private float startDelay;
        
        private TutorialManager tutorialManager;
        private Popup_TouchDisabler touchDisabler;

        protected ExternalTutorialManager(ConfigurationManager configurationManager)
        {
            Configure(configurationManager);
        }

        protected abstract void Configure(ConfigurationManager configurationManager);
        
        public void SetDelays(float startDelay, float stepDelay)
        {
            this.startDelay = startDelay;
            this.stepDelay = stepDelay;
        }

        public void SetTutorialSequence(List<int> tutorialSequence)
        {
            this.tutorialSequence = tutorialSequence;
        }

        public void TryStartTutorials()
        {
            TryGetTutorialManager();
            if (tutorialSequence.Count == 0 || tutorialManager.IsTutorialShowed(tutorialSequence[0]))
                return;
            touchDisabler = Base.gameManager.OpenPopup<Popup_TouchDisabler>();

            ServiceLocator.Find<UnityTimeScheduler>().Schedule(startDelay, StartTutorial, this);
        }

        public void Replay()
        {
            ResetTutorialsShowState();

            touchDisabler = Base.gameManager.OpenPopup<Popup_TouchDisabler>();
            StartTutorial();
        }

        private void StartTutorial()
        {
            StartTutorialStep(0);        
        }

        private void StartTutorialStep(int step)
        {
            if (step >= tutorialSequence.Count)
            {
                touchDisabler.Close();
                touchDisabler = null;
                return;
            }

            tutorialManager.CheckThenShowTutorial(tutorialSequence[step], stepDelay, onHide: () => StartTutorialStep(step + 1));
        }

        private void TryGetTutorialManager()
        {
            if (tutorialManager == null)
                tutorialManager = Base.gameManager.tutorialManager;
        }

        public void ResetTutorialsShowState()
        {
            TryGetTutorialManager();
            foreach (var id in tutorialSequence)
                tutorialManager.SetTutorialState(id, 0);
        }
    }
}


