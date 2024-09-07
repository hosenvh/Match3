using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;


namespace Match3.Main.VideoPlayer
{
    public class ControllerAutoHider : MonoBehaviour
    {
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private Button showButton;

        private const float hideDelay = 3f;
        private const float hideAnimationDuration = 0.3f;
        private Coroutine currentChangeAlphaCoroutine;

        private void Awake()
        {
            showButton.onClick.AddListener(ShowController);
        }

        private void OnEnable()
        {
            ShowController();
        }

        private void ShowController()
        {
            if (currentChangeAlphaCoroutine != null)
                StopAllCoroutines();
            showButton.gameObject.SetActive(false);
            canvasGroup.interactable = true;
            currentChangeAlphaCoroutine = StartCoroutine(ChangeCanvasGroupAlpha(1));
            StartCoroutine(WaitAndHideController());
        }

        private IEnumerator WaitAndHideController()
        {
            yield return new WaitForSeconds(hideDelay);
            HideController();
        }

        private void HideController()
        {
            if (currentChangeAlphaCoroutine != null)
                StopAllCoroutines();
            showButton.gameObject.SetActive(true);
            canvasGroup.interactable = false;
            currentChangeAlphaCoroutine = StartCoroutine(ChangeCanvasGroupAlpha(0));
        }

        private IEnumerator ChangeCanvasGroupAlpha(float targetAlpha)
        {
            float initialAlpha = canvasGroup.alpha;
            float duration = hideAnimationDuration;
            float t = 0;
            while (t < duration)
            {
                t += Time.deltaTime;
                float percent = Mathf.Clamp01(t / duration);
                canvasGroup.alpha = Mathf.Lerp(initialAlpha, targetAlpha, percent);
                yield return null;
            }
        }

        public void ResetWaitAndHideTimer()
        {
            StopWaitAndHideTimer();
            StartWaitAndHideTimer();
        }

        public void StopWaitAndHideTimer()
        {
            StopAllCoroutines();
        }

        public void StartWaitAndHideTimer()
        {
            StartCoroutine(WaitAndHideController());
        }
    }
}