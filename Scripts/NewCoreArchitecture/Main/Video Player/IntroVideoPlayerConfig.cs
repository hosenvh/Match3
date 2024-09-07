using System;
using UnityEngine;


namespace Match3.Main.VideoPlayer
{
    [Serializable]
    public enum VideoPlayerControlOptions
    {
        OnlyControls,
        OnlySkip,
    }

    [Serializable]
    public class IntroVideoPlayerServerConfig
    {
        public VideoPlayerControlOptions controlOptions;
    }

    [Serializable]
    public class IntroVideoPlayerServerCohortConfig : CohortConfigReplacer<IntroVideoPlayerServerConfig>
    {
    }
}