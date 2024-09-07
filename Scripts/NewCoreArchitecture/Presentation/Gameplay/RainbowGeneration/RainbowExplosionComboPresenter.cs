using UnityEngine;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Foundation.Unity.TimeManagement;
using SeganX;
using TMPro;
using UnityEngine.Events;
using RTLTMPro;
using System;
using Match3.Presentation.Gameplay.LogicPresentationHandlers;

namespace Match3.Presentation.Gameplay.RainbowGeneration
{
    public class RainbowExplosionComboPresenter : MonoBehaviour
    {
        public RTLTextMeshPro[] valueTexts;

        public AnimationPlayer animationPlayer;

        private void Awake()
        {
            foreach (var text in valueTexts)
                text.text = "0";
        }
        public void Setup(int comboValue, Action onCompleted)
        {
            foreach(var text in valueTexts)
                text.text = comboValue.ToString();

            animationPlayer.Play();
            this.Wait(2, onCompleted);
        }

    }
}