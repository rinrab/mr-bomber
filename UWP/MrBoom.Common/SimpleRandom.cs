// Copyright (c) Timofei Zhakov. All rights reserved.

using System;

namespace MrBoom.Common
{
    public class SimpleRandom : IRandom
    {
        private readonly Random random;

        public SimpleRandom()
        {
            random = new Random();
        }

        public SimpleRandom(int seed)
        {
            random = new Random(seed);
        }

        public SimpleRandom(Random random)
        {
            this.random = random;
        }

        public int Next(int minValue, int maxValue)
        {
            return random.Next(minValue, maxValue);
        }
    }
}
