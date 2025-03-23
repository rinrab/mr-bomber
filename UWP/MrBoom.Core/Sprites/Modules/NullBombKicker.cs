// Copyright (c) Timofei Zhakov. All rights reserved.

using MrBoom.Core.Sprites.Interface;

namespace MrBoom.Core.Sprites.Modules
{
    public class NullBombKicker : IBombKicker
    {
        public void KickBomb(int x, int y, int dx, int dy)
        {
        }
    }
}
