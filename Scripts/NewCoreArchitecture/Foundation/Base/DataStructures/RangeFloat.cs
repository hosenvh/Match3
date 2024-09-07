using UnityEngine;

namespace Match3.Foundation.Base.DataStructures
{
    [System.Serializable]
    public class RangeFloat
    {
        public float min;
        public float max;

        public float GetRandomInRange()
        {
            return min == max ? min : min + (Random.Range(0, float.MaxValue) % (max - min));
        }
    }
}