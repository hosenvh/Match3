using Match3.Game.Gameplay.Tiles;
using Match3.Presentation.Gameplay.Core;
using Match3.Presentation.Gameplay.Tiles;
using UnityEngine;
using System;

namespace Match3.Presentation.Gameplay.GoalManagement
{
    public class FireflyExitHandler
    {
        FireflyJarPresenter fireflyPresenter;
        GoalTargetGatheringPresentationHandlerImp moveController;

        FireflyJar fireflyJar;

        Action onReached;
        public FireflyExitHandler(FireflyJar fireflyJar, GoalTargetGatheringPresentationHandlerImp moveController)
        {
            this.fireflyJar = fireflyJar;
            this.moveController = moveController;
            fireflyPresenter = fireflyJar.GetComponent<TilePresenter>().As<FireflyJarPresenter>();

            
        }

        public void Handle(Action onReached)
        {
            this.onReached = onReached;
            fireflyPresenter.RegisterOnFireflyExited(OnFireflyExited);
        }

        void OnFireflyExited()
        {
            fireflyPresenter.UnRegisterOnFireflyExited(OnFireflyExited);


            var firefly = GameObject.Instantiate(moveController.fireflyPresenter, moveController.effectContainer, false);
            firefly.transform.position = fireflyPresenter.FireFlyExitPosition();

            moveController.MoveToGoal(fireflyJar, firefly, onReached,  new Vector3(-1,1) * 100);
        }
    }

}