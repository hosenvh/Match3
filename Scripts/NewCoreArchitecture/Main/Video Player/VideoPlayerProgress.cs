using System;
using UnityEngine.ResourceManagement.Exceptions;


namespace Match3.Main.VideoPlayer
{
    public class VideoPlayerProgress
    {
        private UnityEngine.Video.VideoPlayer videoPlayer;

        private ulong TotalFramesCount => videoPlayer.frameCount;
        private long CurrentFrame => videoPlayer.frame;

        public VideoPlayerProgress(UnityEngine.Video.VideoPlayer videoPlayer)
        {
            this.videoPlayer = videoPlayer;
        }

        public float GetProgress()
        {
            return (float) CurrentFrame / TotalFramesCount;
        }

        public void SetProgress(float amount)
        {
            videoPlayer.frame = (long) (amount * TotalFramesCount);
        }

        public bool IsFinished()
        {
            return CurrentFrame >= (long) TotalFramesCount - 1;
        }

        public bool IsStarted()
        {
            return CurrentFrame >= 0;
        }

        public void Pause()
        {
            videoPlayer.Pause();
        }

        public void Play()
        {
            videoPlayer.Play();
        }

        public void StepForward()
        {
            videoPlayer.frame += 48;
        }

        public void StepBackward()
        {
            videoPlayer.frame -= 48;
        }

        public void MoveToEnd()
        {
            videoPlayer.frame = (long) TotalFramesCount;
        }
    }
}