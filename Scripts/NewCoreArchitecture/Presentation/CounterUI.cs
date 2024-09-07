using SeganX;
using System;
using UnityEngine;

namespace Match3.Presentation
{

    public class CounterUI : MonoBehaviour
    {
        public LocalText amountText;

        Func<int> valueFunction;

        int deltaAmount;

        public void Setup(Func<int> valueFunction)
        {
            deltaAmount = 0;
            this.valueFunction = valueFunction;
            UpdateAmount();
        }

        public void UpdateAmount()
        {
            amountText.SetText((valueFunction() + deltaAmount).ToString());
            OnAmountUpdated();
        }

        protected virtual void OnAmountUpdated()
        {

        }

        public void AddDelta(int amount)
        {
            deltaAmount += amount;
            UpdateAmount();
        }

        public void SetDelta(int amount)
        {
            this.deltaAmount = amount;
            UpdateAmount();
        }
    }
}