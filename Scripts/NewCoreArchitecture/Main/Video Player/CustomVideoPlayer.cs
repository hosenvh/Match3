using System;
using System.Collections;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Foundation.Unity.TimeManagement;
using SeganX;
using UnityEngine;
using UnityEngine.Video;
using Object = UnityEngine.Object;


namespace Match3.Main.VideoPlayer
{
    public class CustomVideoPlayer : IVideoPlayer
    {
        private GameObject videoPlayerContainer;
        private UnityEngine.Video.VideoPlayer videoPlayer;
        private VideoPlayerProgress playerProgress;
        private GameState blackUnderlay;
        private Popup_VideoPlayerController controller;

        private Coroutine checkStartCoroutine;
        private Coroutine checkFinishCoroutine;

        public CustomVideoPlayer()
        {
            videoPlayerContainer = new GameObject("Video Player");
            videoPlayer = videoPlayerContainer.AddComponent<UnityEngine.Video.VideoPlayer>();
            videoPlayer.source = VideoSource.Url;
            videoPlayer.targetCamera = Camera.main;
            videoPlayer.renderMode = VideoRenderMode.CameraNearPlane;
            videoPlayer.audioOutputMode = VideoAudioOutputMode.Direct;
            videoPlayer.aspectRatio = VideoAspectRatio.FitVertically;
            videoPlayer.playOnAwake = false;
            videoPlayer.isLooping = false;
            playerProgress = new VideoPlayerProgress(videoPlayer);
            Object.DontDestroyOnLoad(videoPlayerContainer.gameObject);
        }

        public void Play(string videoPath, float timeOut, Action onStarted, Action<bool> onFinished)
        {
            OpenBlackUnderlay();
            OpenControls();

            if (videoPlayer.targetCamera == null)
                videoPlayer.targetCamera = Camera.main;
                
            videoPlayer.url = Application.streamingAssetsPath + "/" + videoPath;
            AddTimeout(timeOut, () => onFinished.Invoke(false));
            AddListeners(onStarted, () => onFinished.Invoke(true));
            videoPlayer.Play();
        }

        public void Stop()
        {
            videoPlayer.Stop();
        }

        public void Dispose()
        {
            Object.Destroy(videoPlayerContainer.gameObject);
        }

        private void AddTimeout(float timeOut, Action onTimeout)
        {
            ServiceLocator.Find<UnityTimeScheduler>().Schedule(timeOut, () =>
            {
                if (videoPlayer == null || HasLoadedVideo())
                    return;
                onTimeout.Invoke();
            }, this);

            bool HasLoadedVideo()
            {
                return playerProgress.IsFinished() || videoPlayer.isPlaying || videoPlayer.isPaused || playerProgress.GetProgress() == 0;
            }
        }

        private void AddListeners(Action onStarted, Action onCompleted)
        {
            checkStartCoroutine = Base.gameManager.StartCoroutine(CheckVideoStarted(onStarted));
            checkFinishCoroutine = Base.gameManager.StartCoroutine(CheckVideoFinished(HandleVideoFinish));

            void HandleVideoFinish()
            {
                if (checkStartCoroutine != null)
                    Base.gameManager.StopCoroutine(checkStartCoroutine);
                CloseControls();
                CloseBlackUnderlay();
                videoPlayer.Stop();
                onCompleted.Invoke();
            }
        }

        private IEnumerator CheckVideoFinished(Action onFinished)
        {
            while (true)
            {
                yield return null;
                if (playerProgress.IsFinished() && checkStartCoroutine == null)
                {
                    onFinished.Invoke();
                    break;
                }
            }
            checkFinishCoroutine = null;
        }

        private IEnumerator CheckVideoStarted(Action onStarted)
        {
            while (true)
            {
                yield return null;
                if (playerProgress.IsStarted())
                {
                    onStarted.Invoke();
                    break;
                }
            }
            checkStartCoroutine = null;
        }

        private void OpenControls()
        {
            var configuration = ServiceLocator.Find<ServerConfigManager>().data;
            var controlOptions = configuration.config.introVideoPlayerServerConfig.controlOptions;
            controller = Base.gameManager.OpenPopup<Popup_VideoPlayerController>().Setup(playerProgress, controlOptions);
        }

        private void CloseControls()
        {
            Base.gameManager.ClosePopup(controller);
        }

        private void OpenBlackUnderlay()
        {
            blackUnderlay = Base.gameManager.OpenPopup<Popup_VideoBlackUnderlay>().Setup(Camera.main);
        }

        private void CloseBlackUnderlay()
        {
            Base.gameManager.ClosePopup(blackUnderlay);
        }
    }
}