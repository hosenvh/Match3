using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SeganX;
using UnityEngine.UI;
using System;
using UnityEngine.Events;
using Match3.Game.Gameplay.SubSystems.LevelEnding;

namespace Match3.Presentation.Gameplay.GoalManagement
{
    public class GoalPresenter : MonoBehaviour
    {
        public Transform targetItemContainer;
        public ParticleSystem collectItemParticle;
        public LocalText remainGoalText = null;
        public GameObject tickGameObject;
        public UnityEvent onGoalReached;
        public UnityEvent resetGoalReached;
             
        GoalTracker goalTracker;

        bool isReached = false;
        int pendingGatherings = 0;

        public void Setup(GameObject goalPrefab, GoalTracker goalTracker)
        {
            this.goalTracker = goalTracker;
            var goal = Instantiate(goalPrefab, targetItemContainer, false);
            goal.SetActive(true);
            remainGoalText.SetText(goalTracker.RemainingAmount().ToString());

            goalTracker.onGatheredAmountIncreasedEvent += UpdateRemainingAmount;
            goalTracker.onGatheredAmountDecreasedEvent += UpdateRemainingAmount;
        }

        private void UpdateRemainingAmount()
        {
            remainGoalText.SetText(RemainingAmount().ToString());

            if (isReached == false && RemainingAmount() <= 0)
            {
                isReached = true;
                onGoalReached.Invoke();
            }
            else if(isReached && RemainingAmount() > 0)
            {
                isReached = false;
                resetGoalReached.Invoke();
            }
        }

        public void PlayGatheringEffect()
        {
            collectItemParticle.Play();
        }

        public void IncreasePendingGatherings()
        {
            pendingGatherings++;
            UpdateRemainingAmount();
        }

        public void DecreasePendingGatherings()
        {
            pendingGatherings--;
            UpdateRemainingAmount();
        }

        private int RemainingAmount()
        {
            return goalTracker.RemainingAmount() + pendingGatherings;
        }

    }
}