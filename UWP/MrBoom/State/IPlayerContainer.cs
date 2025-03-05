// Copyright (c) Timofei Zhakov. All rights reserved.

using System;

namespace MrBoom.State
{
    public interface IPlayerContainer
    {
        bool AddPlayer(Func<int, IPlayerState> providePlayer);
        void Clear();
    }
}
