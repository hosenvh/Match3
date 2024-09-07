using Match3.Foundation.Base.ServiceLocating;
using Match3.Foundation.Unity.TimeManagement;
using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.SubSystems.LevelEnding;
using Match3.Presentation.Gameplay.Core;
using Match3.Presentation.Gameplay.GoalManagement;
using UnityEngine;
using UnityEngine.UI;

namespace Match3.Presentation.Gameplay.LogicPresentationHandlers
{

    public class FinalScorePresentationHandlerImp : MonoBehaviour, FinalScorePresentationHandler
    {
        public float coinMovementSpeed;
        public float initalForceMagnitude;
        public Transform targetTransform;
        public GainedCoinsPresentationController coinsPresentationController;
        public MovingObject coinPrefab;

        public void HandleScoreAddition(int number, Tile tile)
        {
            var pos = tile.Parent().GetComponent<TileStackPresenter>().transform.position;

            var initialForceDirection = new Vector3(-1, 1);

            for (int i = 0; i < number; i++)
            {
                var coin = Instantiate(coinPrefab, targetTransform, false);

                initialForceDirection = Quaternion.Euler(0, 0 , -50 * i) * initialForceDirection;
                coin.Move(
                    pos,
                    coinsPresentationController.ExplosionCoinTargetPosition(),
                    initialForceDirection * initalForceMagnitude,
                    coinMovementSpeed,
                    () => coinsPresentationController.AddCoin(1));
            }


        }
    }
}