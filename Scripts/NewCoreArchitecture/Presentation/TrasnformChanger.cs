using UnityEngine;


namespace Match3.Presentation
{
    public class TrasnformChanger : MonoBehaviour
    {
        public void SetLocalEulerRotationZ(float rotation)
        {
            var rotations = this.transform.localEulerAngles;
            rotations.z = rotation;
            this.transform.localEulerAngles = rotations;
        }

        public void SetLocalScaleY(float scale)
        {
            var scales = this.transform.localScale;
            scales.y = scale;
            this.transform.localScale = scales;
        }
    }
}