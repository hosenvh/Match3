using UnityEngine;


namespace Match3.Foundation.Base
{
    public class BazaarFunctionalityManager : BasicStoreFunctionalityManager
    {
        
        public BazaarFunctionalityManager(string developerId) : base(developerId)
        {
        }
        

        public override long LastAvailableApplicationVersion()
        {
            AndroidJavaClass cafebazaarServiceClass = new AndroidJavaClass("pandascanplay.cafebazaarservice.ServiceManager");

            cafebazaarServiceClass.CallStatic<long>("LastAvailableApplicationVersion");

            return 0;
        }

        protected override void InternalRequestRatingFor(string url)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent");
        AndroidJavaObject intentObject = new AndroidJavaObject("android.content.Intent");

        AndroidJavaClass uriClass = new AndroidJavaClass("android.net.Uri");

        intentObject.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_EDIT"));
        intentObject.Call<AndroidJavaObject>("setData", uriClass.CallStatic<AndroidJavaObject>("parse", "bazaar://details?id="+url));
        intentObject.Call<AndroidJavaObject>("setPackage", "com.farsitel.bazaar");

        AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");
        currentActivity.Call("startActivity", intentObject);
#endif
        }

        protected override void InternalRequestVisitPageFor(string url)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent");
        AndroidJavaObject intentObject = new AndroidJavaObject("android.content.Intent");

        AndroidJavaClass uriClass = new AndroidJavaClass("android.net.Uri");

        intentObject.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_VIEW"));
        intentObject.Call<AndroidJavaObject>("setData", uriClass.CallStatic<AndroidJavaObject>("parse", "bazaar://details?id="+url));
        intentObject.Call<AndroidJavaObject>("setPackage", "com.farsitel.bazaar");

        AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");
        currentActivity.Call("startActivity", intentObject);
#endif
        }

        
        protected override void InternalRequestVisitDeveloperPage(string devId)
        {
#if UNITY_ANDROID && !UNITY_EDITOR 
        AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent");
        AndroidJavaObject intentObject = new AndroidJavaObject("android.content.Intent");

        AndroidJavaClass uriClass = new AndroidJavaClass("android.net.Uri");

        intentObject.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_VIEW"));
        intentObject.Call<AndroidJavaObject>("setData", 
                uriClass.CallStatic<AndroidJavaObject>("parse", "bazaar://collection?slug=by_author&aid=" + devId));
        intentObject.Call<AndroidJavaObject>("setPackage", "com.farsitel.bazaar");

        AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");
        currentActivity.Call("startActivity", intentObject);
#endif
        }
        

        public override string StoreName()
        {
            return "CafeBazaar";
        }

        protected override string StoreBaseURL()
        {
            return "https://cafebazaar.ir/app/";
        }
    }
}