using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Match3.Presentation
{
    public class CommonGUIController : MonoBehaviour
    {
        public CounterUI coinCounter;
        public CounterUI starCounter;

        private void Awake()
        {

            coinCounter.Setup(() => global::Base.gameManager.profiler.CoinCount);
            starCounter.Setup(() => global::Base.gameManager.profiler.StarCount);

            UpdateGUI();
        }

        public void UpdateGUI()
        {
            coinCounter.UpdateAmount();
            starCounter.UpdateAmount();
            //starCountText.SetText(global::Base.gameManager.profiler.StarCount.ToString());
            //coinCountText.SetText(global::Base.gameManager.profiler.CoinCount.ToString());
        }

    }
}