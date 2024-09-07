using System;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Game;
using Match3.Game.NeighborhoodChallenge;
using UnityEngine;
using UnityEngine.Serialization;


namespace Match3.Presentation.HUD
{

    public enum HudType
    {
        Coin,
        Life,
        Key,
        Ticket,
        Star,
        Booster
    }

    public class HudPresentationController : MonoBehaviour
    {

        [FormerlySerializedAs("hudCanvasTransform")] 
        public Transform hudParent;
        public HudElement[] hudElements;
        

        void Awake()
        {
            Init();
        }
        
        
        private void Init()
        {
            foreach (var hudElement in hudElements)
            {
                switch (hudElement.type)
                {
                    case HudType.Coin:
                        hudElement.counter.Setup(GetCoinCount);
                        break;
                    case HudType.Life:
                        hudElement.counter.Setup(GetLifeCount);
                        break;
                    case HudType.Star:
                        hudElement.counter.Setup(GetStarCount);
                        break;
                    case HudType.Key:
                        hudElement.counter.Setup(GetKeyCount);
                        break;
                    case HudType.Ticket:
                        hudElement.counter.Setup(GetTicketCount);
                        break;
                    case HudType.Booster:
                        hudElement.counter.Setup(GetFakeBoosterCount);
                        break;
                }
            }
        }

        
        int GetCoinCount()
        {
            return Base.gameManager.profiler.CoinCount;
        }

        int GetLifeCount()
        {
            return Base.gameManager.profiler.LifeCount;
        }

        int GetStarCount()
        {
            return Base.gameManager.profiler.StarCount;
        }

        int GetTicketCount()
        {
            return ServiceLocator.Find<NeighborhoodChallengeManager>().Ticket().CurrentValue();
        }

        int GetKeyCount()
        {
            return Base.gameManager.profiler.KeyCount;
        }

        int GetFakeBoosterCount()
        {
            return 0;
        }
        
        public HudElement TryGetHudElement(HudType type)
        {
            foreach (var hudElement in hudElements)
            {
                if (hudElement.type == type)
                {
                    return hudElement;
                }
            }

            return null;
        }

        public HudElement TryGetHudElement(Type targetRewardType)
        {
            foreach (var hudElement in hudElements)
            {
                foreach (var suitableReward in hudElement.suitableForRewards)
                {
                    if (targetRewardType == Type.GetType(suitableReward))
                        return hudElement;
                }
            }

            return null;
        }
        
        public void UpdateHud(HudType type)
        {
            TryGetHudElement(type)?.counter.UpdateAmount();
        }
        
        
    
    }

}


