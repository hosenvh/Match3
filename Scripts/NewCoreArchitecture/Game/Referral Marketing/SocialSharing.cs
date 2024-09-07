using System;
using UnityEngine;


namespace Match3.Game.ReferralMarketing.Share
{

    [Serializable]
    public class ShareTarget
    {
        public string androidPackageName;
        public string className;
    }

    [Serializable]
    public class ShareData
    {
        public string title;
        public string subject;
        public string description;
        public string[] filesPath;
    }
    
    
    public static class SocialSharing
    {

        public static void Share(ShareData data, ShareTarget target = null, NativeShare.ShareResultCallback onShareCallBack = null)
        {
            SetLastShareTime();
            
            NativeShare share = new NativeShare();
            
            if (!string.IsNullOrEmpty(data.description)) share.SetText(data.description);
            if (!string.IsNullOrEmpty(data.title)) share.SetTitle(data.title);
            if (!string.IsNullOrEmpty(data.subject)) share.SetSubject(data.subject);
            
            if (data.filesPath != null && data.filesPath.Length > 0)
                foreach (var filePath in data.filesPath)
                    share.AddFile(filePath);

            if (target != null)
                share.AddTarget(target.androidPackageName, target.className);

            if (onShareCallBack != null) share.SetCallback(onShareCallBack);
            
            share.Share();
        }
    
        
        private static void SetLastShareTime()
        {
            PlayerPrefs.SetString("LastShareTime", DateTime.UtcNow.ToFileTimeUtc().ToString());
        }

        public static bool DoesEverSharingHappened()
        {
            return long.Parse(PlayerPrefs.GetString("LastShareTime", "0")) != 0;
        }
        
        public static DateTime GetLastShareTime()
        {
            var fileTime = long.Parse(PlayerPrefs.GetString("LastShareTime", "0"));
            if(fileTime == 0) return DateTime.MaxValue;
            var lastDateTime = DateTime.FromFileTimeUtc(fileTime);
            return lastDateTime;
        }

    }

}


