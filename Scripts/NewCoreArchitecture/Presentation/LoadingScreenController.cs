using Match3.Foundation.Base.Configuration;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Foundation.Unity;
using UnityEngine;
using UnityEngine.UI;


namespace Match3.Presentation.LoadingScreen
{

    public class LoadingScreenController : MonoBehaviour
    {
        public GameObject screenPrefab;
        
        private void Awake()
        {
            ServiceLocator.Find<ConfigurationManager>().Configure(this);
        }

        public void InstantiateScreenPrefab(ResourceGameObjectAsset splashScreen)
        {
            screenPrefab = Instantiate(splashScreen.Load(), transform);
        }
        
    }
    
}    
    

