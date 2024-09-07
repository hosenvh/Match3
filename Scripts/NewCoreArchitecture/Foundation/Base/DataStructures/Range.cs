using System;

namespace Match3.Foundation.Base.DataStructures
{
    public class Range<T> 
    {
        public readonly T start;
        public readonly T end;

        public Range(T start, T end)
        {
            this.start = start;
            this.end = end;
        }
    }

    public static class RangeExtensions
    {
        public static bool Contains<T>(this Range<T> self, T value) where T : IComparable
        {
            return self.start.CompareTo(value) <= 0 && self.end.CompareTo(value) >= 0;
        }
    }

}