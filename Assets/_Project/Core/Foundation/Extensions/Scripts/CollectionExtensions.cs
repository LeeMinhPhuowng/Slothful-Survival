using System;
using System.Collections.Generic;
using System.Threading;

namespace Core.Foundation.Extensions
{


    public static class CollectionExtensions
    {
        private static readonly ThreadLocal<Random> _threadLocalRandom = new(() => new Random());

        private static Random GetRandom() => _threadLocalRandom.Value!;

        public static T RandomElement<T>(this IReadOnlyList<T> list)
        {
            if (list == null)
            {
                throw new ArgumentNullException(nameof(list));
            }

            if (list.Count == 0)
            {
                throw new InvalidOperationException("Cannot get random element from an empty collection.");
            }

            return list[GetRandom().Next(list.Count)];
        }

        public static void Shuffle<T>(this IList<T> list)
        {
            if (list == null)
            {
                throw new ArgumentNullException(nameof(list));
            }

            var rng = GetRandom();
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = rng.Next(i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }

        public static bool IsNullOrEmpty<T>(this IReadOnlyCollection<T> collection)
        {
            return collection == null || collection.Count == 0;
        }
    }
}
