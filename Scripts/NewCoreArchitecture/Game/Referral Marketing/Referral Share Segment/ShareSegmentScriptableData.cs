using System.Collections;
using System.Collections.Generic;
using Match3.Data;
using UnityEngine;
using UnityEngine.Serialization;


namespace Match3.Game.ReferralMarketing.Segments
{

    public class ShareSegmentScriptableData : ScriptableObject
    {
        public bool isEnable = true;
        public string tag;
        
        [Space(10)]
        public bool isLimited;
        public int dailyQuota;

        [Space(10)] 
        public SelectableReward reward;
    }

}
