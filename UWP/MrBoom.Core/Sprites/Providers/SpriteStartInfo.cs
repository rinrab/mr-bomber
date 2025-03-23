// Copyright (c) Timofei Zhakov. All rights reserved.

namespace MrBoom.Core.Sprites.Providers
{
    public class SpriteStartInfo
    {
        public int X { get; }
        public int Y { get; }
        public int DefaultSpeed { get; }
        public int LivesCount { get; }
        public SpriteType Type { get; }
        public int SubType { get; }

        public SpriteStartInfo(int x, int y, int defaultSpeed, int livesCount, SpriteType type, int subType)
        {
            X = x;
            Y = y;
            DefaultSpeed = defaultSpeed;
            LivesCount = livesCount;
            Type = type;
            SubType = subType;
        }
    }
}
