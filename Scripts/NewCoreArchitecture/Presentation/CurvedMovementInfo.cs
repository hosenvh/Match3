using System;
using DG.Tweening;
using UnityEngine.Serialization;

namespace Match3.Presentation
{
    [Serializable]
    public struct CurvedMovementInfo
    {
        public float speed;
        public float force;
        public float forceAngle;
        [FormerlySerializedAs("movementCurve")]
        public Ease curve;
    }
}