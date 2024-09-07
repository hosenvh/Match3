using System;
using UnityEngine;
using System.Collections;

namespace Match3.Presentation.Gameplay.LogicPresentationHandlers
{
    public static class MonobehaviorExtensions
    {

        public static void Wait(this MonoBehaviour monoBehaviour, float delay, Action callBack)
        {
            monoBehaviour.StartCoroutine(WaitCoroutine(delay, callBack));
        }

        static IEnumerator WaitCoroutine(float delay, Action callBack)
        {
            yield return new WaitForSeconds(delay);
            callBack();
        }
    }
}