using System;
using System.Collections.Generic;
using Match3.Game.Gameplay;
using Match3.Game.Gameplay.SubSystems.LevelEnding;
using UnityEngine;
using UnityEngine.Events;

namespace Match3.Presentation.Gameplay.GoalManagement
{

    public class GoalsPresentationController : MonoBehaviour
    {
        public GoalPresenter goalCounterPrefab = null;
        public GoalTypePrefabDatabase goalTypePrefabDatabase;
        public RectTransform goalsContainer;

        public UnityEvent onDisappear;

        Dictionary<GoalTargetType, GoalPresenter> goalPresenters = new Dictionary<GoalTargetType, GoalPresenter>();

        Action onDisappearCompleted;

        public void Setup(GameplayController gpc)
        {

            foreach (var goalInfo in gpc.GetSystem<LevelStoppingSystem>().AllGoals())
                SetupGoalCounterFor(goalInfo);
        }

        void SetupGoalCounterFor(GoalTracker goalTracker)
        {
            var presenter = Instantiate(goalCounterPrefab, goalsContainer, false);
            presenter.Setup(goalTypePrefabDatabase.PrefabFor(goalTracker.goalType), goalTracker);
            goalPresenters.Add(goalTracker.goalType, presenter);
        }

        public GoalPresenter GoalPresenterFor(GoalTargetType goalTargetType)
        {
            return goalPresenters[goalTargetType];
        }



        public void Disappear(Action onCompleted)
        {
            this.onDisappearCompleted = onCompleted;
            onDisappear.Invoke();
        }

        public void OnDisappearCompleted()
        {
            onDisappearCompleted();
            onDisappearCompleted = null;
        }
    }
}