using System;
using Match3.Game.TaskManagement;




namespace Match3.Presentation.WorldMap
{
    public class WorldMapTutorialMainMenuTask : MainMenuTask
    {
        private int tutorialIndex;

        public WorldMapTutorialMainMenuTask(int tutorialIndex)
        {
            this.tutorialIndex = tutorialIndex;
        }
        
        protected override void InternalExecution(Action onComplete, Action onAbort)
        {
            Base.gameManager.tutorialManager.CheckThenShowTutorial(tutorialIndex, 0, null, onAbort);
        }

        protected override bool IsConditionSatisfied()
        {
            return true;
        }
    }    
}


