using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Foundation.Unity.TimeManagement;

namespace Match3.Presentation.Gameplay.LevelConditionManagement
{
    public class ExtraMoveFlowEffect : MonoBehaviour
    {
        public ParticleSystem effectParticelSystem;
        public float movementSpeed;
        public float startDelay;
        public float destructionDelay;

        public void Play(Vector3 from, Vector3 to, Action onReached)
        {
            this.transform.position = from;
            this.transform.
                DOMove(to, movementSpeed).
                SetSpeedBased(true).
                SetDelay(startDelay).
                OnComplete(() => HandleReaching(onReached));
        }

        void HandleReaching(Action onReached)
        {
            onReached();
            effectParticelSystem.Stop();
            ServiceLocator.Find<UnityTimeScheduler>().Schedule(destructionDelay, () => Destroy(this.gameObject), this);
        }

        private void OnDestroy()
        {
            ServiceLocator.Find<UnityTimeScheduler>().UnSchedule(this);
        }

    }
}