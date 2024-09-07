using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace KitchenParadise.Utiltiy.Base
{
    public static class CollectionExtensions
    {
        

        // WARNING: Using general Foreach will probably create garbage.
        public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action)
        {
            foreach (var e in collection)
                action(e);
        }

        public static void ForEach<T>(this HashSet<T> collection, Action<T> action)
        {
            foreach (var e in collection)
                action(e);
        }

        public static bool IsEmpty<T>(this ICollection<T> collection)
        {
            return collection.Count == 0;
        }

        public static T Find<T>(this ICollection<T> collection, Func<T,bool> condition)
        {
            foreach (var e in collection)
                if (condition(e) == true)
                    return e;

            return default(T);
        }

        public static int CollectionHashCode<T>(this ICollection<T> collection)
        {
            var hash = 0;
            foreach (var e in collection)
                hash += e.GetHashCode();

            hash += collection.Count.GetHashCode() + 10;

            return hash;
        }

        public static T RandomElement<T>(this IList<T> list)
        {
            UnityEngine.Debug.Assert(list.Count > 0);
            var random = UnityEngine.Random.Range(0, list.Count);

            return list[random];
        }

        public static T MaxElement<T,U>(this List<T> list, Func<T, U> selector) where U : IComparable
        {
            var listCount = list.Count;

            if (listCount == 0)
                return default(T);

            var maxElem = list[0];
            var maxValue = selector(maxElem);
            for (int i = 0; i < listCount; ++i)
            {
                T elem = list[i];
                if (selector(elem).CompareTo(maxValue) > 0)
                {
                    maxElem = elem;
                    maxValue = selector(elem);
                }
            }

            return maxElem;
        }

        public static T MinElement<T,U>(this ICollection<T> list, Func<T, U> selector) where U: IComparable
        {
            if (list.Count == 0)
                return default(T);

            var minElem = list.First();
            var minValue = selector(minElem);
            foreach (var elem in list)
            {
                if (selector(elem).CompareTo(minValue) < 0)
                {
                    minElem = elem;
                    minValue = selector(elem);
                }
            }

            return minElem;
        }

        public static T Last<T>(this IList<T> list)
        {
            return list[list.Count - 1];
        }

        // TODO: Refactor this.
        public static HashSet<T> Except<T>(this HashSet<T> set, HashSet<T> otherSet )
        {
            var remaining = new HashSet<T>(set);
            remaining.ExceptWith(otherSet);

            return remaining;
        }

        public static void AddRange<T>(this HashSet<T> set, ICollection<T> elements)
        {
            set.UnionWith(elements);
        }

        public static T FirstElement<T>(this HashSet<T> set)
        {
            var e = set.GetEnumerator();
            e.MoveNext();
            return e.Current;
        }

        public static void SetCapacity<T>(this HashSet<T> set, int capacity) where T : new()
        {
            for (int i = 0; i < capacity; ++i)
                set.Add(new T());

            set.Clear();
        }

        public static void SetCapacity<T>(this HashSet<T> set, int capacity, Func<T> factory)
        {
            for (int i = 0; i < capacity; ++i)
                set.Add(factory());

            set.Clear();
        }

        public static bool Has<TTarget>(this IEnumerable list)
        {
            foreach (var element in list)
                if (element is TTarget target)
                    return true;
            return false;
        }

        public static TTarget Get<TTarget>(this IEnumerable list)
        {
            foreach (var element in list)
                if (element is TTarget target)
                    return target;
            return default;
        }

        public static void DoRotateShift<T>(this List<T> list)
        {
            if(list.Count == 0)
                return;
            T first = list[0];
            list.RemoveAt(0);
            list.Add(first);
        }

        public static void DoForEachElement<T>(this List<T> list, Action<T> todo)
        {
            foreach (T element in list)
                todo.Invoke(element);
        }

        public static IEnumerable<TSource> FindDuplicates<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector, IEqualityComparer<TKey> comparer)
        {
            var hash = new HashSet<TKey>(comparer);
            return source.Where(item => !hash.Add(selector(item))).ToList();
        }

        public static IEnumerable<TSource> FindDuplicates<TSource>(this IEnumerable<TSource> source, IEqualityComparer<TSource> comparer)
        {
            return source.FindDuplicates(x => x, comparer);
        }

        public static IEnumerable<TSource> FindDuplicates<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector)
        {
            return source.FindDuplicates(selector, null);
        }

        public static IEnumerable<TSource> FindDuplicates<TSource>(this IEnumerable<TSource> source)
        {
            return source.FindDuplicates(x => x, null);
        }
    }
}