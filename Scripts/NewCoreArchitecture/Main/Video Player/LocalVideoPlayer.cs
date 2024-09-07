using System;
using UnityEngine;



namespace Match3.Main.VideoPlayer
{

    public class LocalVideoPlayer : IVideoPlayer
    {
        private string preloadVideoPath;
        
        public void Play(string videoPath, float timeOut, Action onStarted, Action<bool> onFinished)
        {
            onStarted();
            
            Handheld.PlayFullScreenMovie(
                videoPath,
                Color.black,
                FullScreenMovieControlMode.Minimal,
                FullScreenMovieScalingMode.AspectFit);
            
            onFinished(true);
        }

        public void PreLoad(string videoPath)
        {
            preloadVideoPath = videoPath;
        }

        public void PlayPreLoadedVideo(float timeOut, Action onStarted, Action<bool> onFinished)
        {
            if (string.IsNullOrEmpty(preloadVideoPath))
            {
                Debug.LogError("Local Video Player Has No Preloaded Video");
                return;
            }
            
            Play(preloadVideoPath, timeOut, onStarted, onFinished);
        }

        public void Stop()
        {
            // Nothing for this
        }

        public void Dispose()
        {
            // nothing for this
        }
    }

}