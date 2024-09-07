using SeganX;
using System;
using UnityEngine.Events;

namespace Match3.Presentation
{
    public class TimedCounterUI : CounterUI
    {
        public LocalText timerText;
        public UnityEvent onTimerActivated;
        public UnityEvent onTimerDeactivated;

        Func<int> remainingTimeFunction;
        Func<bool> isTimerActiveFunction;

        bool shoudDisplayTimer;

        public void Setup(Func<int> valueFunction, Func<int> remainingTimeFunction, Func<bool> isTimerActiveFunction)
        {
            this.remainingTimeFunction = remainingTimeFunction;
            this.isTimerActiveFunction = isTimerActiveFunction;
            base.Setup(valueFunction);

            SetShouldDisplayTimer(isTimerActiveFunction());

        }

        private void SetShouldDisplayTimer(bool shoudDisplay)
        {
            this.shoudDisplayTimer = shoudDisplay;

            if (shoudDisplay)
                onTimerActivated.Invoke();
            else
                onTimerDeactivated.Invoke();
        }

        protected override void OnAmountUpdated()
        {
            SetShouldDisplayTimer(isTimerActiveFunction());
        }

        private void Update()
        {
            if (shoudDisplayTimer)
            {
                if (isTimerActiveFunction() == false)
                    SetShouldDisplayTimer(false);

                var remainingTime = remainingTimeFunction();
                timerText.SetText(string.Format("{0:00}:{1:00}", remainingTime / 60, remainingTime % 60));
            }
        }
    }
}