


using Medrick.Foundation.Base.PlatformFunctionality;
using UnityEngine;

namespace KitchenParadise.Foundation.Unity.PlatformFunctionality
{
    public class AndroidPlatformFunctionalityManager : PlatformFunctionalityManager
    {
        public int VersionCode()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            AndroidJavaClass contextCls = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject context = contextCls.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject packageMngr = context.Call<AndroidJavaObject>("getPackageManager");
            string packageName = context.Call<string>("getPackageName");
            AndroidJavaObject packageInfo = packageMngr.Call<AndroidJavaObject>("getPackageInfo", packageName, 0);
            return packageInfo.Get<int>("versionCode");
#elif UNITY_ANDROID && UNITY_EDITOR
            return UnityEditor.PlayerSettings.Android.bundleVersionCode;
#else
            return -1;
#endif
        }
    }
}