using Match3.Foundation.Base.ServiceLocating;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Match3.Foundation.Unity.TimeManagement
{
    // WARNING: This does not support multiple scheduling of same callback.
    // TODO: Fixed the above warning.
    public class UnityTimeScheduler : MonoBehaviour, Service
    {
        Dictionary<Action, IEnumerator> scheduledCoroutines = new Dictionary<Action, IEnumerator>(10);
        Dictionary<Action, object> ownerRelations = new Dictionary<Action, object>(10);


        public void Schedule(float delay, Action callback, object owner)
        {
            var coroutine = DelayedCall(delay, callback);
            scheduledCoroutines[callback] = coroutine;
            ownerRelations[callback] = owner;

            base.StartCoroutine(coroutine);
        }

        public void UnSchedule(Action callback)
        {
            if (scheduledCoroutines.ContainsKey(callback) == false)
                return;

            StopCoroutine(scheduledCoroutines[callback]);
            ownerRelations.Remove(callback);
            scheduledCoroutines.Remove(callback);
        }

        // TODO: Reduce the garbage generation.
        public void UnSchedule(object owner)
        {
            var actionCopy = new List<Action>(scheduledCoroutines.Keys);
            foreach (var callback in actionCopy)
                if (callback.Target == owner)
                    UnSchedule(callback);

            var ownerRelationsCopy = new Dictionary<Action, object>(ownerRelations);
            foreach (var entry in ownerRelationsCopy)
                if (entry.Value == owner)
                    UnSchedule(entry.Key);
        }

        IEnumerator DelayedCall(float delay, Action callback)
        {
            yield return new WaitForSeconds(delay);
            scheduledCoroutines.Remove(callback);
            ownerRelations.Remove(callback);
            callback();
        }

        public new Coroutine StartCoroutine(IEnumerator routine)
        {
            return base.StartCoroutine(routine);
        }
    }
}