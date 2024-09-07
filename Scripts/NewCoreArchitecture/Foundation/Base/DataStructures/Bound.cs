using System;


namespace Match3.Foundation.Base.DataStructures
{
    [Serializable]
    public class Bound<T>
    {
        public T min;
        public T max;
    }

    public static class BoundExtensions
    {
        public static bool Contains<T>(this Bound<T> self, T value) where T : IComparable
        {
            return self.min.CompareTo(value) <= 0 && self.max.CompareTo(value) >= 0;
        }
    }
}