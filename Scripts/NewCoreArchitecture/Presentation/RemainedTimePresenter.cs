using System;
using System.Collections;
using Match3.Presentation.TextAdapting;
using SeganX;
using UnityEngine;


namespace Match3.Presentation
{
    public class RemainedTimePresenter : MonoBehaviour
    {
        public Action OnTimerFinished = delegate { };
        public bool IsTimerStarted { get; private set; }

        [SerializeField] private TextAdapter detailedTimerText;
        [SerializeField] private TextAdapter generalTimerText;

        private Func<TimeSpan> getRemainingTimeFunc;

        public void ForceStop()
        {
            Stop();
        }

        public void StartTimer(Func<TimeSpan> getRemainingTimeFunc)
        {
            IsTimerStarted = true;
            EnableTimers();
            this.getRemainingTimeFunc = getRemainingTimeFunc;

            if (IsTimeLeft())
                StartCoroutine(TimerUpdating());
            else
                Stop();

            void EnableTimers()
            {
                SetTimersEnableState(isEnable: true);
            }
        }


        private IEnumerator TimerUpdating()
        {
            var waitTime = new WaitForSeconds(1);
            while (IsTimeLeft())
            {
                UpdateTimer();
                yield return waitTime;
            }
            Stop();
        }

        private bool IsTimeLeft()
        {
            return getRemainingTimeFunc().TotalSeconds > 0;
        }

        private void UpdateTimer()
        {
            TimeSpan remainingTime = getRemainingTimeFunc();

            bool showGeneralTime = remainingTime.TotalDays > 1d;

            generalTimerText.gameObject.SetActive(showGeneralTime);
            detailedTimerText.gameObject.SetActive(!showGeneralTime);

            if (showGeneralTime)
                generalTimerText.SetText(Utilities.GetLocalizedFormattedTime(TrimHours((int) remainingTime.TotalSeconds), showZeros: false));
            else
                detailedTimerText.SetText(Utilities.GetFormatedTime((int) remainingTime.TotalSeconds, forceShowHours: true));
        }

        private int TrimHours(int totalSeconds)
        {
            totalSeconds += 43200;
            return totalSeconds - (totalSeconds % 86400);
        }

        private void Stop()
        {
            IsTimerStarted = false;

            DisableTimers();
            StopAllCoroutines();
            OnTimerFinished.Invoke();

            void DisableTimers()
            {
                SetTimersEnableState(isEnable: false);
            }
        }

        private void SetTimersEnableState(bool isEnable)
        {
            generalTimerText.gameObject.SetActive(isEnable);
            detailedTimerText.gameObject.SetActive(isEnable);
        }
    }
}