using System;
using UnityEngine;
using System.Collections;

namespace Match3.Presentation.Gameplay.LogicPresentationHandlers
{
    public class PiggyBankCoinRewardHandler : MonoBehaviour
    {
        [SerializeField] private float coinMovementSpeed;
        [SerializeField] private float initalForceMagnitude;
        [SerializeField] private Transform startTransform;
        [SerializeField] private Transform targetTransform;
        [SerializeField] private Transform movingCoinsContainer;
        [SerializeField] private MovingObject coinPrefab;

        private Action onRewardingFinished;
        private Action onEachCoinInstantiated;
        private Action onEachCoinReached;
        private int movingCoinCount = 0;

        public void HandleReward(int movingCoinCount, Action onEachCoinInstantiated, Action onEachCoinReached, Action onRewardingFinished)
        {
            this.onRewardingFinished = onRewardingFinished;
            this.onEachCoinInstantiated = onEachCoinInstantiated;
            this.onEachCoinReached = onEachCoinReached;
            this.movingCoinCount = movingCoinCount;
            StartCoroutine(MoveCoins());
        }

        private IEnumerator MoveCoins()
        {
            var wait = new WaitForSeconds(0.5f);
            var moveCount = movingCoinCount;
            var initialForceDirection = Quaternion.Euler(10, 300, 0) * new Vector3(-0.2f, 1);
            var initialForce = initialForceDirection * initalForceMagnitude;
            while (moveCount > 0)
            {
                yield return wait;
                moveCount--;
                var coin = Instantiate(coinPrefab, movingCoinsContainer, false);
                onEachCoinInstantiated.Invoke();
                coin.Move(
                    startTransform.position,
                    targetTransform.position,
                    initialForce,
                    coinMovementSpeed,
                    OnCoinReached);
            }
        }

        private void OnCoinReached()
        {
            onEachCoinReached.Invoke();
            CountRewards();
        }

        private void CountRewards()
        {
            movingCoinCount--;
            if (movingCoinCount <= 0)
                onRewardingFinished.Invoke();
        }
    }
}