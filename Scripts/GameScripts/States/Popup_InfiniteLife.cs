using Match3.Foundation.Base.ServiceLocating;
using SeganX;
using System;
using System.Collections;
using System.Collections.Generic;
using Match3.Presentation.TextAdapting;
using UnityEngine;
using UnityEngine.UI;

namespace Match3
{
    public class Popup_InfiniteLife : GameState
    {
        [SerializeField]
        LocalTextAdapter timerText = null;
        Action OnCloseCallback = null;
        public Popup_InfiniteLife Setup(Action OnCloseCallback)
        {
            this.OnCloseCallback = OnCloseCallback;
            InvokeRepeating("UpdateTimer", 0, 1.0f);
            return this;
        }


        void OnDisable()
        {
            CancelInvoke("UpdateTimer");
        }

        void UpdateTimer()
        {
            var infiniteLifeTime = ServiceLocator.Find<ILife>().GetInInfiniteLifeRemainingTime();
            timerText.SetText(Utilities.GetFormatedTime(infiniteLifeTime));
            if (infiniteLifeTime <= 0)
            {
                Back();
            }
        }

        public override void Back()
        {
            base.Back();
            OnCloseCallback();
        }

    }
}