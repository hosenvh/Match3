using System;
using SeganX;
using UnityEngine;



namespace Match3
{

    public class KeyHudController : MonoBehaviour
    {
        
        [Space(10)] [SerializeField] private LocalText keyCountText = default;
        
        private void Start()
        {
            UpdateKeyHud();
        }

        public void UpdateKeyHud()
        {
            keyCountText.SetText(Base.gameManager.profiler.KeyCount.ToString());
        }

        public void OpenKeyShop()
        {
            
        }
        
    }

}


