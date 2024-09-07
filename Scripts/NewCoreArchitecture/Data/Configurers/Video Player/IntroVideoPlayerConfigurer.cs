using System;
using I2.Loc;
using Match3.Foundation.Base.Configuration;
using Match3.Foundation.Unity.Configuration;
using Match3.Main.VideoPlayer;
using UnityEngine;
using PandasCanPlay.HexaWord.Utility;


namespace Match3.Data.Configuration
{

    [CreateAssetMenu(menuName = "Match3/Configurations/IntroVideoConfigurer")]
    public class IntroVideoPlayerConfigurer : ScriptableConfiguration, Configurer<IntroVideoPlayer>
    {
        [TypeAttribute(typeof(IVideoPlayer), false)]
        public string videoPlayer;
        public LocalizedStringTerm introVideoLocalizedPath;
        
        public float timeOut;
        
        [Header("Fallback Setting")]
        public bool needFallbackPlayer;
        [TypeAttribute(typeof(IVideoPlayer), false)]
        public string fallbackVideoPlayer;
        public LocalizedStringTerm introFallbackVideoLocalizedPath;
        
        
        
        public void Configure(IntroVideoPlayer entity)
        {
            entity.SetVideoPath(introVideoLocalizedPath);
            entity.CreatePlayer(Type.GetType(videoPlayer));

            if (needFallbackPlayer)
            {
                entity.CreateFallbackPlayer(Type.GetType(fallbackVideoPlayer));
                entity.SetFallbackVideoPath(introFallbackVideoLocalizedPath);
            }
            
            entity.SetTimeOut(timeOut);
        }

        public override void RegisterSelf(ConfigurationManager manager)
        {
            manager.Register(this);
        }
    }

}