// Copyright (c) Timofei Zhakov. All rights reserved.

using System;

namespace MrBoom
{
    [Flags]
    public enum SoundEffectType
    {
        Bang = 2 << 0,
        PoseBomb = 2 << 1,
        Sac = 2 << 2,
        Pick = 2 << 3,
        PlayerDie = 2 << 4,
        Oioi = 2 << 5,
        Ai = 2 << 6,
        Addplayer = 2 << 7,
        Victory = 2 << 8,
        Draw = 2 << 9,
        Clock = 2 << 10,
        TimeEnd = 2 << 11,
        Skull = 2 << 12
    }
}
