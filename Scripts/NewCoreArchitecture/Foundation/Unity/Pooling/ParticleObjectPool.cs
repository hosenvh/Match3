using System;
using UnityEngine;

namespace Match3.Foundation.Unity.ObjectPooling
{
    public class  ParticleObjectPool : UnityObjectPool<ParticleSystem>
    {

        protected override void PreSetup()
        {
            base.PreSetup();
            internalPool.SetCreationPostProcess(AttachStoppingListener);
        }

        private void AttachStoppingListener(ParticleSystem particleSystem)
        {
            var main = particleSystem.main;
            main.stopAction = ParticleSystemStopAction.Callback;
            var listener = particleSystem.gameObject.AddComponent<ParticleSystemStoppingListener>();

            listener.SetOnStopAction(() => Release(particleSystem));
        }
    }
}