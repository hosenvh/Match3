using Match3.Foundation.Base.EventManagement;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Game.Gameplay.SubSystems.PowerUpManagement;
using System;
using System.Collections.Generic;
using Match3.Profiler;
using UnityEngine;

namespace Match3.Presentation.Gameplay.PowerUpActivation
{
    public class PowerUpContainer : MonoBehaviour, EventListener
    {


        public PowerUpSelectionInputHandler inputController;

        List<PowerUpActivator> powerUpActivators;


        public void Setup(GameplayState gameState )
        {
            inputController.Setup(gameState);

            powerUpActivators = new List<PowerUpActivator>(GetComponentsInChildren<PowerUpActivator>());
            foreach (var activator in powerUpActivators)
            {
                activator.Setup(inputController, gameState.gameplayController);
            }
            ServiceLocator.Find<EventManager>().Register(this);
        }

        void OnDestroy()
        {
            ServiceLocator.Find<EventManager>().UnRegister(this);
        }


        public void OnEvent(GameEvent evt, object sender)
        {
            if (evt is HammerPowerUpActivatedEvent)
                DecrementPowerUpCountFor(0);
            else if (evt is BroomPowerUpActivatedEvent)
                DecrementPowerUpCountFor(1);
            else if (evt is HandPowerUpActivatedEvent)
                DecrementPowerUpCountFor(2);


        }

        private void DecrementPowerUpCountFor(int powerUpIndex)
        {
            if (DoesReservedRewardExist())
                ConsumeReservedReward();
            else
                Base.gameManager.profiler.ChangePowerupCount(powerUpIndex, -1);
            // WARNING: The Decrement must not be applied here.

            foreach (var activator in powerUpActivators)
                activator.UpdateState();

            bool DoesReservedRewardExist()
            {
                LevelReservedRewardsHandler profiler = Base.gameManager.levelSessionProfiler.PowerUpsReservedRewardsHandler;
                Type rewardType= RewardConversionUtility.GetPowerUpRewardTypeFromPowerUpIndex(powerUpIndex);

                return profiler.DoesRewardOfTypeExist(rewardType);
            }

            void ConsumeReservedReward()
            {
                LevelReservedRewardsHandler profiler = Base.gameManager.levelSessionProfiler.PowerUpsReservedRewardsHandler;
                Type rewardType= RewardConversionUtility.GetPowerUpRewardTypeFromPowerUpIndex(powerUpIndex);
                
                profiler.ConsumeReservedReward(rewardType);
            }
        }
    }
}