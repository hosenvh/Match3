using Match3.Game.Gameplay;
using Match3.Game.Gameplay.SubSystems.LevelEnding;
using UnityEngine;


namespace Match3.Presentation.Gameplay.LevelConditionManagement
{
    public abstract class StopConditionPresenter : MonoBehaviour
    {
        public abstract void Setup(StopConditinon stopCondition);
    }

    public class StoppingConditionPresentationController : MonoBehaviour
    {
        public MovementStopConditionPresenter movementStopConditionPresenter;
        public TimeStopConditionPresenter timeStopConditionPresenter;

        public void Setup(GameplayController gpc)
        {

            StopConditionPresenter presenter = null;

            foreach (var stopCondition in gpc.GetSystem<LevelStoppingSystem>().StopConditions())
            {
                if (stopCondition is MovementStopCondition)
                    presenter = movementStopConditionPresenter;
                else if (stopCondition is TimeStopCondition)
                    presenter = timeStopConditionPresenter;

                if(presenter != null)
                {
                    presenter.gameObject.SetActive(true);
                    presenter.Setup(stopCondition);
                    break;
                }
            }



            //var movementStopCondition =.As<MovementStopCondition>();

            //movementStopCondition.onMovementChanged += UpdateRemainMoveGui;

            //UpdateRemainMoveGui(movementStopCondition.RemainingMovements());
        }
    }
}