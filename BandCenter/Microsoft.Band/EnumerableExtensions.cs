// Decompiled with JetBrains decompiler
// Type: Microsoft.Band.EnumerableExtensions
// Assembly: Microsoft.Band, Version=1.3.20517.1, Culture=neutral, PublicKeyToken=null
// MVID: AFCBFE03-E2CF-481D-86F4-92C60C36D26A
// Assembly location: C:\Users\Pdawg\Downloads\Microsoft Band Sync Setup\Microsoft_Band.dll

using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Band
{
    internal static class EnumerableExtensions
    {
        public static IEnumerable<T> Concat<T>(this IEnumerable<T> first, T additional)
        {
            foreach (T obj in first)
                yield return obj;
            yield return additional;
        }

        public static IEnumerable<T> Concat<T>(T first, T second)
        {
            yield return first;
            yield return second;
        }

        public static IEnumerable<T> Concat<T>(T first, IEnumerable<T> additional)
        {
            yield return first;
            foreach (T obj in additional)
                yield return obj;
        }

        public static IEnumerable<T> TakeAddDefaults<T>(this IEnumerable<T> items, int count, T defaultValue = default)
        {
            foreach (T obj in items.Take(count))
            {
                --count;
                yield return obj;
            }
            while (count-- > 0)
                yield return defaultValue;
        }

        public static IEnumerable<T> AsEnumerable<T>(this T item)
        {
            yield return item;
        }
    }
}
