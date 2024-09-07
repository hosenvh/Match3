using System;
using Match3.Foundation.Base;
using PandasCanPlay.HexaWord.Utility;
using UnityEngine;


namespace Match3.Data.Configuration
{
    [Serializable]
    public class MarketInitializerConfig
    {
        [TypeAttribute(typeof(MarketManager), false)]
        [SerializeField] private string marketType = "";
        [TypeAttribute(typeof(MarketManager), false)]
        [SerializeField] private string fallbackMarketType = "";
        [SerializeField] private bool shouldUseFallbackMarket;
        [TypeAttribute(typeof(StoreFunctionalityManager), false)]
        [SerializeField] private string marketFunctionalityType = "";
        [SerializeField] private string marketDeveloperId = "";
        [SerializeField] private bool showVideoAdConfirmBox = false;

        public Type MarketType => Type.GetType(marketType);
        public Type FallbackMarketType => Type.GetType(fallbackMarketType);
        public bool ShouldUseFallbackMarket => shouldUseFallbackMarket;
        public Type MarketFunctionalityType => Type.GetType(marketFunctionalityType);
        public string MarketDeveloperId => marketDeveloperId;
        public bool ShowVideoAdConfirmBox => showVideoAdConfirmBox;
    }
}