using Match3.Foundation.Base.EventManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Match3
{
    public class SocialNetworkFreeCoinEvent : GameEvent
    {
        public string analyticsKey = "";
        public int amount = 0;
    }

    public class SocialNetworkFreeRewardGivingEvent : GameEvent
    {
        public readonly string socialName;

        public SocialNetworkFreeRewardGivingEvent(string socialName)
        {
            this.socialName = socialName;
        }
    }

    public class SocialNetworkFreeRewardGivingStartedEvent : SocialNetworkFreeRewardGivingEvent
    {
        public SocialNetworkFreeRewardGivingStartedEvent(string socialName) : base(socialName)
        {
        }
    }

    public class SocialNetworkFreeRewardGivingFinishedEvent : SocialNetworkFreeRewardGivingEvent
    {
        public SocialNetworkFreeRewardGivingFinishedEvent(string socialName) : base(socialName)
        {
        }
    }
}