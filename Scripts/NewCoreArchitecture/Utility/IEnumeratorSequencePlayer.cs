using System;
using System.Collections;
using System.Collections.Generic;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Foundation.Unity.TimeManagement;
using UnityEngine;


namespace Match3.Utility
{
    public class IEnumeratorSequence
    {
        private readonly List<IEnumerator> sequence;

        public IEnumeratorSequence(List<IEnumerator> sequence)
        {
            this.sequence = sequence;
        }

        public IEnumerator PlaySequence()
        {
            UnityTimeScheduler coroutinePlayer = ServiceLocator.Find<UnityTimeScheduler>();
            Coroutine[] coroutines = new Coroutine[sequence.Count];
            for (int i = 0; i < coroutines.Length; i++)
                coroutines[i] = coroutinePlayer.StartCoroutine(sequence[i]);
            foreach (Coroutine coroutine in coroutines)
                yield return coroutine;
        }
    }

    public class IEnumeratorSequencePlayer
    {
        private readonly Queue<IEnumeratorSequence> animationSequences = new Queue<IEnumeratorSequence>();
        private readonly UnityTimeScheduler coroutinePlayer;
        private Coroutine runningCoroutine;

        public bool IsRunningSequence => runningCoroutine != null;

        public IEnumeratorSequencePlayer()
        {
            coroutinePlayer = ServiceLocator.Find<UnityTimeScheduler>();
        }

        public void AddSequence(IEnumerator coroutine)
        {
            animationSequences.Enqueue(new IEnumeratorSequence(new List<IEnumerator>(){coroutine}));
        }

        public void AddSequence(IEnumeratorSequence sequence)
        {
            animationSequences.Enqueue(sequence);
        }

        public void AddAction(Action action)
        {
            animationSequences.Enqueue(new IEnumeratorSequence(new List<IEnumerator>(){ PlayAction(action) }));
        }

        public void AddDelay(float delay)
        {
            animationSequences.Enqueue(new IEnumeratorSequence(new List<IEnumerator>(){ Wait(delay) }));
        }

        public void PlayAllSequences(Action onComplete)
        {
            runningCoroutine = coroutinePlayer.StartCoroutine(PlayCoroutinesSequential(onComplete));
        }

        public void ResetSequencePlayer()
        {
            StopCoroutine();
            animationSequences.Clear();
            runningCoroutine = null;
        }

        public void StopCoroutine()
        {
            if (runningCoroutine == null)
                return;
            coroutinePlayer.StopCoroutine(runningCoroutine);
        }

        private IEnumerator PlayCoroutinesSequential(Action onComplete)
        {
            while (animationSequences.Count != 0)
                yield return coroutinePlayer.StartCoroutine(animationSequences.Dequeue().PlaySequence());
            onComplete.Invoke();
            runningCoroutine = null;
        }

        private IEnumerator PlayAction(Action action)
        {
            action.Invoke();
            yield break;
        }

        private IEnumerator Wait(float seconds)
        {
            yield return new WaitForSeconds(seconds);
        }
    }
}