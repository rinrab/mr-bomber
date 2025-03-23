// Copyright (c) Timofei Zhakov. All rights reserved.

namespace MrBoom.Common
{
    public class UnrepeatableRandom : IRandom
    {
        private readonly IRandom random;

        private int? last = null;

        public UnrepeatableRandom(IRandom random)
        {
            this.random = random;
        }

        public UnrepeatableRandom()
        {
            random = new SimpleRandom();
        }

        public int Next(int min, int max)
        {
            if (max - min <= 1)
            {
                return min;
            }
            else
            {
                while (true)
                {
                    int val = random.Next(min, max);
                    if (val != last)
                    {
                        last = val;
                        return val;
                    }
                }
            }
        }
    }
}
