using System;
using Match3.Game;
using PandasCanPlay.HexaWord.Utility;
using UnityEngine;


namespace Match3.Presentation.HUD
{

    [RequireComponent(typeof(CounterUI))]
    public class HudElement: MonoBehaviour
    {
        public HudType type;
        [Type(typeof(Reward), false)]
        public string[] suitableForRewards;
        public Transform hudPosition;
        public CounterUI counter;

        private void Reset()
        {
            counter = transform.GetComponent<CounterUI>();
        }
    }

}