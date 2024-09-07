using System.Collections;
using Match3.Game.Effects.GamePlayEffects;
using UnityEngine;
using Random = UnityEngine.Random;


namespace Match3.Presentation.Effects.PresentationEffects
{
    public class GameplayCameraShakePresentation : MonoBehaviour, GameplayCameraShakePresentationPort
    {
        [SerializeField] private RectTransform boardGame;

        private Vector3 originalLocalPosition;

        private void Start()
        {
            originalLocalPosition = boardGame.localPosition;
        }

        public void ShakeCamera(float duration)
        {
            StartCoroutine(PlayCameraShake(shakeDuration: duration));
        }

        private IEnumerator PlayCameraShake(float shakeDuration)
        {
            float elapsedTime = 0f;
            while (elapsedTime < shakeDuration)
            {
                boardGame.localPosition = originalLocalPosition + Random.insideUnitSphere;
                yield return null;
                elapsedTime += Time.deltaTime;
            }

            boardGame.localPosition = originalLocalPosition;
        }
    }
}