using System;
using System.Collections.Generic;
using Match3.Overlay.Advertisement.Players.Base;
using Match3.Overlay.Advertisement.Service.ChoosingPolicy.Base;
using PandasCanPlay.HexaWord.Utility;
using UnityEngine;


namespace Match3.Overlay.Advertisement.Players.Data
{
    [Serializable]
    public class AdvertisementPlayersDataContainer
    {
        [Serializable]
        public class AdvertisementPlayerData
        {
            [Serializable]
            public class AdvertisementPlayerPlatformData
            {
                public string appId;
                public string rewardedZoneId;
                public string interstitialZoneId;
            }

            [SerializeField] [Type(typeof(AdvertisementPlayer), includeAbstracts: false, showPartialName: true)]
            public string playerType;
            public bool isEnable;
            public bool shouldInitializeOnApplicationStart;
            public int chance;
            public int priority;
            public int dailyMaxPlaysCount;

            public AdvertisementPlayerPlatformData platformData;
            public Type PlayerType => Type.GetType(playerType);
        }

        [SerializeField] [Type(typeof(AdvertisementPlayerChoosingPolicy), includeAbstracts: false, showPartialName: true)]
        private string choosingPolicyType = "";

        public List<AdvertisementPlayerData> adPlayers = new List<AdvertisementPlayerData>();
        public Type ChoosingPolicyType => Type.GetType(choosingPolicyType);
    }
}