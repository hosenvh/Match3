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

    public class YoutubeVideoPlayer : IVideoPlayer
    {
        private YoutubePlayer youtubePlayer;

        private bool onFinishedEventCalled = false;
        private bool preloadVideo = false;

        private GameState blackUnderlay;
        
        public YoutubeVideoPlayer()
        {
            var youtubePlayerObject = new GameObject("YoutubeVideoPlayer");
            var videoPlayer = youtubePlayerObject.AddComponent<UnityEngine.Video.VideoPlayer>();
            videoPlayer.source = VideoSource.Url;
            videoPlayer.targetCamera = Camera.main;
            videoPlayer.renderMode = VideoRenderMode.CameraNearPlane;
            videoPlayer.audioOutputMode = VideoAudioOutputMode.Direct;
            videoPlayer.aspectRatio = VideoAspectRatio.FitHorizontally;
            videoPlayer.playOnAwake = false;
            
            youtubePlayer = youtubePlayerObject.AddComponent<YoutubePlayer>();
            youtubePlayer.videoPlayer = videoPlayer;
            youtubePlayer.audioPlayer = videoPlayer;
            youtubePlayer.videoQuality = YoutubeSettings.YoutubeVideoQuality.STANDARD;
            youtubePlayer.videoFormat = YoutubeSettings.VideoFormatType.MP4;
            youtubePlayer.autoPlayOnStart = false;
            youtubePlayer.autoPlayOnEnable = false;
            youtubePlayer.Stop();
        }

        public void Play(string videoUrl, float timeOut, Action onStarted, Action<bool> onFinished)
        {
            OpenBlackUnderlay();
            
            onFinishedEventCalled = false;
            SetListenersAndTimeout(timeOut, onStarted, onFinished);
            youtubePlayer.Play(videoUrl);
        }

        public void PreLoad(string videoPath)
        {
            Object.DontDestroyOnLoad(youtubePlayer.gameObject);
            youtubePlayer.PreLoadVideo(videoPath);
            preloadVideo = true;
        }

        public void PlayPreLoadedVideo(float timeOut, Action onStarted, Action<bool> onFinished)
        {
            if (!preloadVideo)
            {
                Debug.LogError("Youtube Video Player Has No Preloaded Video");
                return;
            }

            OpenBlackUnderlay();

            if (youtubePlayer.videoPlayer.targetCamera == null)
                youtubePlayer.videoPlayer.targetCamera = Camera.main;
            
            onFinishedEventCalled = false;
            SetListenersAndTimeout(timeOut, onStarted, onFinished);
            youtubePlayer.Play();
        }
        
        
        public void Stop()
        {
            youtubePlayer.Stop();
        }

        public void Dispose()
        {
            Object.Destroy(youtubePlayer.gameObject);
        }
        
        
        private void SetListenersAndTimeout(float timeOut, Action onStarted, Action<bool> onFinished)
        {
            var startCheckingCoroutine = Base.gameManager.StartCoroutine(CheckingVideoStarted(onStarted));

            youtubePlayer.OnVideoFinished.AddListener(() =>
            {
                if (onFinishedEventCalled) return;

                onFinishedEventCalled = true;
                OnFinishVideo(true);
            } );

            ServiceLocator.Find<UnityTimeScheduler>().Schedule(timeOut, () =>
            {
                if (onFinishedEventCalled || IsVideoReallyStarted()) return;
                OnFinishVideo(false);
            }, this);

            void OnFinishVideo(bool result)
            {
                CloseBlackUnderlay();
                youtubePlayer.Stop();
                Base.gameManager.StopCoroutine(startCheckingCoroutine);
                onFinished(result);
            }
        }

        IEnumerator CheckingVideoStarted(Action onStarted)
        {
            while (true)
            {
                yield return null;
                if (IsVideoReallyStarted())
                {
                    onStarted();
                    break;
                }
            }
        }

        private bool IsVideoReallyStarted()
        {
            return youtubePlayer.videoPlayer.frame >= 0;
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