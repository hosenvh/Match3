using Match3.Foundation.Base.ServiceLocating;
using Match3.Foundation.Unity.TimeManagement;
using System;
using UnityEngine;

namespace Match3.Presentation.Gameplay.LogicPresentationHandlers
{
    public class MultiTargetSequencialEffectPlayer: MonoBehaviour
    {

        public float initalDelay;
        public float intervalDelay;


        Counter counter;

        Action<int> playStepEffectAction;

        bool forceStop;
        int count;


        public void Play(int count, Action<int> playStepEffectAction,  Action onSequenceCompleted)
        {
            if(count <= 0)
            {
                onSequenceCompleted();
                return;
            }

            forceStop = false;
            this.count = count;
            this.playStepEffectAction = playStepEffectAction;

            this.counter = new Counter(count, onSequenceCompleted);

            ServiceLocator.Find<UnityTimeScheduler>().Schedule(initalDelay, () => PlayStep(0), this);
        }

        private void PlayStep(int currentIndex)
        {

            playStepEffectAction(currentIndex);
            counter.Decrement();

            if (forceStop)
                return;
            else if (currentIndex + 1 < count)
                ServiceLocator.Find<UnityTimeScheduler>().Schedule(intervalDelay, () => PlayStep(currentIndex + 1), this);
        }


        void OnDestroy()
        {
            ServiceLocator.Find<UnityTimeScheduler>().UnSchedule(this);
        }


        public void StopPlacing()
        {
            forceStop = true;
        }


    }
}