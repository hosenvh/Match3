using System;
using UnityEngine;
using UnityEngine.Video;
using Object = UnityEngine.Object;
using unityVideoPlayer = UnityEngine.Video.VideoPlayer;

namespace Match3.Main.VideoPlayer
{

    public class OnlineVideoPlayer : IVideoPlayer
    {

        private unityVideoPlayer videoPlayer;
        private string videoPath;
        
        public OnlineVideoPlayer()
        {
            var youtubePlayerObject = new GameObject("YoutubeVideoPlayer");
            videoPlayer = youtubePlayerObject.AddComponent<unityVideoPlayer>();
            videoPlayer.source = VideoSource.Url;
            videoPlayer.targetCamera = Camera.main;
            videoPlayer.renderMode = VideoRenderMode.CameraNearPlane;
            videoPlayer.audioOutputMode = VideoAudioOutputMode.Direct;
            videoPlayer.aspectRatio = VideoAspectRatio.FitHorizontally;
            videoPlayer.playOnAwake = false;
            
            videoPlayer.Prepare();
        }
        
        public void Play(string videoPath, float timeOut, Action onStarted, Action<bool> onFinished)
        {
            videoPlayer.url = videoPath;
            videoPlayer.Play();
        }

        public void PreLoad(string videoPath)
        {
            Object.DontDestroyOnLoad(videoPlayer.gameObject);
            
            videoPlayer.url = videoPath;
            videoPlayer.Prepare();
        }

        public void PlayPreLoadedVideo(float timeOut, Action onStarted, Action<bool> onFinished)
        {
            videoPlayer.Play();
        }

        public void Stop()
        {
            videoPlayer.Stop();
        }

        public void Dispose()
        {
            Object.Destroy(videoPlayer.gameObject);
        }
    }

}