using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Match3.Presentation.Gameplay.PowerUpActivation
{
    public class HammerPowerUpEffect : MonoBehaviour
    {

        public Animation effectAnimation;

        Action onComeplted;

        public void Play(Action onCompleted)
        {
            this.onComeplted = onCompleted;
            effectAnimation.Play();
        }

        public void Hit()
        {
            onComeplted();
        }

        public void Destory()
        {
            Destroy(this.gameObject);
        }
    }
}