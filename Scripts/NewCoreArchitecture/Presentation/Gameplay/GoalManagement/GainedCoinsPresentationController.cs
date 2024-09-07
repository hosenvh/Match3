using System;
using Match3.Game.Gameplay;
using Match3.Game.Gameplay.SubSystems.LevelEnding;
using SeganX;
using UnityEngine;
using UnityEngine.Events;

namespace Match3.Presentation.Gameplay.GoalManagement
{
    public class GainedCoinsPresentationController : MonoBehaviour
    {
        public LocalText coinsText;

        public UnityEvent onCoindAdded;
        public UnityEvent onAppear;

        public Transform explosionCoinTarget;

        public Action onAppearingCompleted;

        LevelFinalScoringSystem levelFinalScoringSystem;

        int currentAmount;
        private bool currentAmountFinalized = false;

        public void Setup(GameplayController gameplayController)
        {
            levelFinalScoringSystem = gameplayController.GetSystem<LevelFinalScoringSystem>();
            UpdateCoins();

            //this.gameObject.SetActive(true);
        }

        public void Appear(Action onCompleted)
        {
            this.gameObject.SetActive(true);
            this.onAppearingCompleted = onCompleted;
            onAppear.Invoke();
        }

        public void AppearingCompleted()
        {
            onAppearingCompleted();
            onAppearingCompleted = null;
        }


        public void AddCoin(int amount)
        {
            if(currentAmountFinalized == false)
            {
                currentAmount += amount;
                UpdateAmountText();
            }
            onCoindAdded.Invoke();
        }

        private void UpdateAmountText()
        {
            coinsText.SetText(currentAmount.ToString());
        }

        public Vector3 ExplosionCoinTargetPosition()
        {
            return explosionCoinTarget.transform.position;
        }

        public void UpdateCoins(bool isFinal = false)
        {
            currentAmount = levelFinalScoringSystem.CurrentScore();
            UpdateAmountText();
            if (isFinal)
                currentAmountFinalized = isFinal;
        }
    }
}