using System;
using UnityEngine;

namespace Match3.Foundation.Unity.ObjectPooling
{
    public class ParticleSystemStoppingListener : MonoBehaviour
    {
        Action stopAction;

        public void SetOnStopAction(Action action)
        {
            stopAction = action;
        }

        private void OnParticleSystemStopped()
        {
            stopAction.Invoke();
        }
    }
}