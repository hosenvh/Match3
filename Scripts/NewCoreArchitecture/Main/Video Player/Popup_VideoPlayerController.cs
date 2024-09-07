using System;
using SeganX;
using UnityEngine;
using UnityEngine.UI;


namespace Match3.Main.VideoPlayer
{
    public class Popup_VideoPlayerController : GameState
    {
        [SerializeField] private Button pauseButton;
        [SerializeField] private Button playButton;
        [SerializeField] private Button stepForwardButton;
        [SerializeField] private Button stepBackwardButton;
        [SerializeField] private Button skipButton;
        [SerializeField] private VideoProgressPresentation progressPresentation;
        [SerializeField] private ControllerAutoHider autoHider;

        private bool isInitialized;

        private VideoPlayerProgress videoPlayerProgress;

        public Popup_VideoPlayerController Setup(VideoPlayerProgress videoPlayerProgress, VideoPlayerControlOptions controlOptions)
        {
            this.videoPlayerProgress = videoPlayerProgress;
            SetupProgressPresentation();
            AddListeners();
            HandleControlOptions();
            isInitialized = true;
            return this;

            void SetupProgressPresentation()
            {
                progressPresentation.Setup(onUpdateIndicatorPosition: videoPlayerProgress.SetProgress,
                                           onPointerDown: () =>
                                           {
                                               videoPlayerProgress.Pause();
                                               autoHider.StopWaitAndHideTimer();
                                           },
                                           onPointerUp: () =>
                                           {
                                               videoPlayerProgress.Play();
                                               autoHider.StartWaitAndHideTimer();
                                           });
            }

            void AddListeners()
            {
                pauseButton.onClick.AddListener(HandlePausePlay);
                pauseButton.onClick.AddListener(autoHider.ResetWaitAndHideTimer);
                playButton.onClick.AddListener(HandlePausePlay);
                playButton.onClick.AddListener(autoHider.ResetWaitAndHideTimer);
                stepForwardButton.onClick.AddListener(videoPlayerProgress.StepForward);
                stepForwardButton.onClick.AddListener(autoHider.ResetWaitAndHideTimer);
                stepBackwardButton.onClick.AddListener(videoPlayerProgress.StepBackward);
                stepBackwardButton.onClick.AddListener(autoHider.ResetWaitAndHideTimer);
                skipButton.onClick.AddListener(videoPlayerProgress.MoveToEnd);
            }

            void HandleControlOptions()
            {
                switch (controlOptions)
                {
                    case VideoPlayerControlOptions.OnlyControls:
                        skipButton.gameObject.SetActive(false);
                        break;
                    case VideoPlayerControlOptions.OnlySkip:
                        progressPresentation.gameObject.SetActive(false);
                        playButton.gameObject.SetActive(false);
                        pauseButton.gameObject.SetActive(false);
                        stepForwardButton.gameObject.SetActive(false);
                        stepBackwardButton.gameObject.SetActive(false);
                        break;
                }
            }
        }

        private void HandlePausePlay()
        {
            if (!pauseButton.gameObject.activeInHierarchy)
            {
                videoPlayerProgress.Play();
                pauseButton.gameObject.SetActive(true);
                playButton.gameObject.SetActive(false);
            }
            else
            {
                videoPlayerProgress.Pause();
                playButton.gameObject.SetActive(true);
                pauseButton.gameObject.SetActive(false);
            }
        }

        private void Update()
        {
            if (isInitialized == false)
                return;
            if (videoPlayerProgress == null)
            {
                isInitialized = false;
                return;
            } 
            progressPresentation.UpdateProgress(videoPlayerProgress.GetProgress());
        }

        public override void Back()
        {
            if (IsOpenedFromSettingPopup()) // TODO: Improve This
                videoPlayerProgress.MoveToEnd();

            bool IsOpenedFromSettingPopup()
            {
                return gameManager != null && gameManager.GetPopup<Popup_Settings>() != null;
            }
        }
    }
}