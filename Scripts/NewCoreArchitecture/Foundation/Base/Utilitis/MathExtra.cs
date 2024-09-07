
using System;
using System.Collections.Generic;
using System.Linq;

namespace KitchenParadise.Utiltiy.Base
{
    public static class MathExtra 
    {
        static public int WeightedRandomIndex(List<double> weights)
        {
            double totalWeights = 0;
            weights.ForEach((w) => totalWeights += w);

            double random = UnityEngine.Random.value * totalWeights;

            for (int i = 0; i < weights.Count; i++)
            {
                if (weights[i] >= random)
                    return i;
                else
                    random -= weights[i];
            }
            return -1;
        }

        static public int WeightedRandomIndex(List<float> weights)
        {
            float totalWeights = 0;

            for (int i = 0; i < weights.Count; ++i)
                totalWeights += weights[i];

            float random = UnityEngine.Random.value * totalWeights;

            for (int i = 0; i < weights.Count; ++i)
            {
                if (weights[i] >= random && weights[i] > 0)
                    return i;
                else
                    random -= weights[i];
            }
            UnityEngine.Debug.LogError("Couldn't find an index");
            return UnityEngine.Random.Range(0, weights.Count - 1);
        }

        static public T WeightedRandomElement<T>(ICollection<T> items, Func<T,float> weightFunc)
        {
            var itemsList = items.ToList();
            var weights = itemsList.Select(t => weightFunc(t)).ToList();

            return itemsList[WeightedRandomIndex(weights)];
        }


        public static float Clamp(float value, float min, float max)
        {
            return UnityEngine.Mathf.Clamp(value, min, max);
        }

        // TODO: Merge with below
        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            Random rnd = new Random();
            while (n > 1)
            {
                int k = (rnd.Next(0, n) % n);
                n--;
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public static void Shuffle<T>(this IList<T> list, Random rnd)
        {
            int n = list.Count;
            while (n > 1)
            {
                int k = (rnd.Next(0, n) % n);
                n--;
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public static IList<T> Swap<T>(this IList<T> list, int indexA, int indexB)
        {
            T tmp = list[indexA];
            list[indexA] = list[indexB];
            list[indexB] = tmp;
            return list;
        }
    }
}