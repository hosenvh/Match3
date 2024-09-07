using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Core.PathCore;
using DG.Tweening.Plugins.Options;
using UnityEngine;

namespace KitchenParadise.Presentation
{
    public static class DoTweenUtilities
    {
        public static TweenerCore<Vector3, Path, PathOptions> DoMoveWithForce(this Transform target, float force, float forceAngle , Vector2 endValue, float duration, bool followPath)
        {
            Vector2 startValue = target.position;

            var distance = Vector2.Distance(startValue, endValue);

            var currentGlobalAngle = CalculateAngle(endValue, startValue);

            var globalDirection = 270 <= currentGlobalAngle || currentGlobalAngle <=90 ?  ZDirection(currentGlobalAngle + forceAngle) : ZDirection(currentGlobalAngle - forceAngle);

            var firstHandle = startValue + globalDirection * force * distance;
            var secondHandle = endValue + Vector2.up * force;

            Vector3[] positions = { endValue, firstHandle, secondHandle };

            var doTween = target.DOPath(positions, duration, PathType.CubicBezier, PathMode.Full3D).
                SetOptions(AxisConstraint.None, AxisConstraint.Z);

            if (followPath)
                doTween.SetLookAt(0.01f, -globalDirection);

            return doTween;
        }

        public static float CalculateAngle(Vector3 from, Vector3 to)
        {
            var dir = from - to;
            var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            return angle < 0 ? angle + 360 : angle;

        }

        static Vector2 ZDirection(float zDeg)
        {
            float zRad = zDeg * Mathf.Deg2Rad;
            float fX = Mathf.Sin(zRad);
            float fY = Mathf.Cos(zRad);
            return new Vector2(fY, fX);
        }
    }
}