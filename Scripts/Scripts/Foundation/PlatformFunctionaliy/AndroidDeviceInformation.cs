
using Match3.Foundation.Base.ServiceLocating;
using UnityEngine;

namespace Medrick.Foundation.Base.PlatformFunctionality
{
    public class AndroidDeviceInformation : Service
    {
        private string googleAdID = "-1";

        public AndroidDeviceInformation()
        {
            Application.RequestAdvertisingIdentifierAsync((adId, trackingEnabled, error) =>
            {
                googleAdID = adId;
            });
        }

        public string AndroidID()
        {
            var androidID = "-1";
#if UNITY_ANDROID && !UNITY_EDITOR

            AndroidJavaClass clsUnity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject objActivity = clsUnity.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject objResolver = objActivity.Call<AndroidJavaObject>("getContentResolver");
            AndroidJavaClass clsSecure = new AndroidJavaClass("android.provider.Settings$Secure");
            androidID = clsSecure.CallStatic<string>("getString", objResolver, "android_id");
#endif
            return androidID;
        }

        public string GoogleAdID()
        {
            return googleAdID;
        }

        public string DeviceID()
        {
            return SystemInfo.deviceUniqueIdentifier;
        }
    }
}