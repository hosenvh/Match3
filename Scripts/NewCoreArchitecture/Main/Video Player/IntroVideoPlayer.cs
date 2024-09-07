using System;
using Match3.Foundation.Base.Configuration;
using Match3.Foundation.Base.ServiceLocating;
using I2.Loc;
using UnityEngine;


namespace Match3.Main.VideoPlayer
{

    public interface IVideoPlayer
    {
        void Play(string videoPath, float timeOut, Action onStarted, Action<bool> onFinished);
        void Stop();
        void Dispose();
    }

    public class IntroVideoPlayer
    {
        private LocalizedStringTerm videoPath;
        private LocalizedStringTerm fallbackVideoPath;
        private IVideoPlayer videoPlayer;
        private IVideoPlayer fallbackPlayer;
        private float timeOut;

        public IntroVideoPlayer()
        {
            ServiceLocator.Find<ConfigurationManager>().Configure(this);
        }

        public void Play(Action onStarted, Action<bool> onFinished, bool disposePlayerOnFinish)
        {
            videoPlayer.Play(videoPath, timeOut, onStarted, ManipulatedOnFinished);          
            
            void ManipulatedOnFinished(bool result)
            {
                if (result == false && HasFallbackPlayer())
                    fallbackPlayer.Play(fallbackVideoPath, timeOut, onStarted, ManipulatedOnFinishedForFallbackPlayer);
                else
                    onFinished(result);
                
                if(disposePlayerOnFinish) DisposePlayer();
            }
            
            void ManipulatedOnFinishedForFallbackPlayer(bool result)
            {
                onFinished(result);
                if(disposePlayerOnFinish) DisposeFallbackPlayer();
            }
        }

        public void SetVideoPath(LocalizedStringTerm videoPath)
        {
            this.videoPath = videoPath;
        }

        public void SetFallbackVideoPath(LocalizedStringTerm videoPath)
        {
            fallbackVideoPath = videoPath;
        }
        
        public void CreatePlayer(Type type)
        {
            videoPlayer = Activator.CreateInstance(type) as IVideoPlayer;
        }

        public void CreateFallbackPlayer(Type type)
        {
            fallbackPlayer = Activator.CreateInstance(type) as IVideoPlayer;
        }
        
        public void SetTimeOut(float timeOut)
        {
            this.timeOut = timeOut;
        }

        private void DisposePlayer()
        {
            videoPlayer.Dispose();
        }
        
        private void DisposeFallbackPlayer()
        {
            fallbackPlayer.Dispose();
        }
        
        private bool HasFallbackPlayer()
        {
            return fallbackPlayer != null;
        }
    }
    
    
}


