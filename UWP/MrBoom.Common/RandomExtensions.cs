// Copyright (c) Timofei Zhakov. All rights reserved.

using System;
using System.Collections.Generic;

namespace MrBoom.Common
{
    public static class RandomExtensions
    {
        public static int Next(this IRandom random, int maxValue)
        {
            return random.Next(0, maxValue);
        }

        public static void Shuffle<T>(this IRandom random, IList<T> items)
        {
            int count = items.Count;

            for (int i = 0; i < count - 1; i++)
            {
                int j = random.Next(i, count);

                if (j != i)
                {
                    (items[j], items[i]) = (items[i], items[j]);
                }
            }
        }

        public static T NextEnum<T>(this IRandom random) where T : Enum
        {
            var values = Enum.GetValues(typeof(T));

            return (T)values.GetValue(random.Next(values.Length));
        }

        public static T NextElement<T>(this IRandom random, IList<T> items)
        {
            int index = random.Next(items.Count);

            return items[index];
        }
    }
}
