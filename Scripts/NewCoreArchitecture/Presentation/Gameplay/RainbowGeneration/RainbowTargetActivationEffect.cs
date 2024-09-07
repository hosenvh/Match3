using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Foundation.Unity.TimeManagement;
using Match3.Game.Gameplay.Tiles;

namespace Match3.Presentation.Gameplay.RainbowGeneration
{
    public class RainbowTargetActivationEffect : MonoBehaviour
    {
        public float movementSpeed;
        public ParticleSystem trailParticleSystem;
        public ParticleSystem hoveringParticleSystem;
        public ParticleSystem explosionParticle;
        public TrailRenderer trailRenderer;

        public void Setup(Vector3 targetPosition, Color trailColor)
        {
            SetRainbowTrailColor(trailColor);
            trailParticleSystem.gameObject.SetActive(true);
            this.transform.DOMove(targetPosition, movementSpeed).SetSpeedBased().SetEase(Ease.Linear).OnComplete(ActivateHovering);
        }

        private void ActivateHovering()
        {
            trailParticleSystem.gameObject.SetActive(false);
            hoveringParticleSystem.gameObject.SetActive(true);
        }

        public void Explode()
        {
            hoveringParticleSystem.gameObject.SetActive(false);
            explosionParticle.gameObject.SetActive(true);
            ServiceLocator.Find<UnityTimeScheduler>().Schedule(explosionParticle.main.duration, () => Destroy(this.gameObject), this);
        }

        private void OnDestroy()
        {
            ServiceLocator.Find<UnityTimeScheduler>().UnSchedule(this);
        }

        public void SetRainbowTrailColor(Color color)
        {
            trailRenderer.material.SetColor("_TintColor", color);
        }
    }
}