using System;
using UnityEngine;
using DG.Tweening;

namespace Match3.Presentation.Gameplay
{
    public class SeekingRocketPresenter : MonoBehaviour
    {
        public float initialForce;

        public void Move(Vector3 targetPosition, float movementSpeed, Action onReached)
        {
            this.gameObject.SetActive(true);


            var forwardDir = ZDirection(transform.eulerAngles.z);

            var firstHandle = this.transform.position +  (forwardDir * initialForce);
            var secondHandle = targetPosition + Vector3.up * initialForce;

            Vector3[] positions = { targetPosition, firstHandle, secondHandle};

            this.transform.DOPath(positions, movementSpeed, PathType.CubicBezier, PathMode.Full3D).
                SetOptions(AxisConstraint.None, AxisConstraint.Z).
                SetSpeedBased(true).
                SetLookAt(0.01f, -forwardDir).
                SetEase(Ease.InSine).
                OnComplete(() => { Destroy(this.gameObject); onReached(); });
        }

        Vector3 ZDirection(float zDeg)
        {
            float zRad = zDeg * Mathf.Deg2Rad;
            float fX = Mathf.Sin(zRad);
            float fY = Mathf.Cos(zRad);
            return  new Vector3(fY, fX, 0);
        }
    }
}