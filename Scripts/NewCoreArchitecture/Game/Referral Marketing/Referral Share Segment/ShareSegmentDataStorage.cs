using System;
using Match3.Game.ReferralMarketing.Segments;
using UnityEngine;

namespace Match3.Game.ReferralMarketing
{
    public class ShareSegmentDataStorage
    {
        public void SetSegmentDailyQuota(ShareSegment segment, int count)
        {
            PlayerPrefs.SetInt(SegmentDailyQuotaKey(segment), count);
        }

        public void SetSegmentLastTimeShared(ShareSegment segment, DateTime time)
        {
            PlayerPrefs.SetString(SegmentLastTimeSharedKey(segment), time.ToShortDateString());
        }
        
        public int GetSegmentDailyQuota(ShareSegment segment)
        {
            return PlayerPrefs.GetInt(SegmentDailyQuotaKey(segment), segment.dailyQuota);
        }

        public DateTime GetLastTimeSegmentShared(ShareSegment segment)
        {
            var dateTimeString = PlayerPrefs.GetString(SegmentLastTimeSharedKey(segment), DateTime.Now.ToShortDateString());
            return Convert.ToDateTime(dateTimeString);
        }
        

        private string SegmentDailyQuotaKey(ShareSegment segment)
        {
            return segment.GetType().Name + "_availableCount";
        }

        private string SegmentLastTimeSharedKey(ShareSegment segment)
        {
            return segment.GetType().Name + "_lastTimeShared";
        }
    }
}