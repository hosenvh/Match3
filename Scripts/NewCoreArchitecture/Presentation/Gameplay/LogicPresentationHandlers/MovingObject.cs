#pragma warning disable CS0109
using System;
using UnityEngine;
using DG.Tweening;

namespace Match3.Presentation.Gameplay.LogicPresentationHandlers
{

    // TODO: Find a better name
    public class MovingObject : MonoBehaviour
    {

        public Ease ease;
        public new AnimationCurve animation;
        public float deathDelay;
        public float speed;


        public void Move(Vector3 startPos, Vector3 endPos, Vector3 initialForce, float speed, Action onReachedAction)
        {
            this.speed = speed;
            this.transform.position = startPos;

            var forcePos = startPos + initialForce;

            Vector3[] position = { endPos, forcePos, endPos };

            this.transform.DOPath(position, speed, PathType.CubicBezier).
                SetSpeedBased(true).
                SetEase(ease).
                OnComplete(
                () => OnReached(onReachedAction) );

        }

        void OnReached(Action onReachedAction)
        {

            onReachedAction();
            if (deathDelay <= 0)
                Destroy(this.gameObject);
            else
                this.Wait(deathDelay, () => Destroy(this.gameObject));


        }


    }
}