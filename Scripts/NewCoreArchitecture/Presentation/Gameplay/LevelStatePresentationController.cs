using Match3.Game.Gameplay;
using Match3.Presentation.Gameplay.LevelConditionManagement;
using Match3.Presentation.Gameplay.GoalManagement;
using Match3.Presentation.Gameplay.RainbowGeneration;
using SeganX;
using UnityEngine;


namespace Match3.Presentation.Gameplay
{
   
    public class LevelStatePresentationController : MonoBehaviour
    {
        public GoalsPresentationController goalsPresentationController;
        public StoppingConditionPresentationController stoppingConditionPresentationController;
        public RainbowGeneratorPresenter rainbowGeneratorPresenter;

        public LocalText levelIndexText = null;

        public LevelStatePresentationController Setup(GameplayController gpc, int levelIndex)
        {
            levelIndexText.SetText((levelIndex + 1).ToString());



            goalsPresentationController.Setup(gpc);
            stoppingConditionPresentationController.Setup(gpc);
            rainbowGeneratorPresenter.Setup(gpc);

            return this;
        }
    }
}