using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TwolipsDating.Utilities
{
    public static class DictionaryHelper
    {
        public static IEnumerable<TValue> UniqueRandomValues<TKey, TValue>(IDictionary<TKey, TValue> dict, int? seed = null)
        {
            Random rand = seed.HasValue ? new Random(seed.Value) : new Random();
            Dictionary<TKey, TValue> values = new Dictionary<TKey, TValue>(dict);
            while (values.Count > 0)
            {
                TKey randomKey = values.Keys.ElementAt(rand.Next(0, values.Count));  // hat tip @yshuditelu
                TValue randomValue = values[randomKey];
                values.Remove(randomKey);
                yield return randomValue;
            }
        }

    }
}