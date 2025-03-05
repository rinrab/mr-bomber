// Copyright (c) Timofei Zhakov. All rights reserved.

using System.Collections.Generic;

namespace MrBoom.State
{
    public interface IPlayerProvider
    {
        IPlayerState this[int index] { get; }

        int Count { get; }

        IEnumerable<IPlayerState> EnumeratePlayers();
        IEnumerable<IPlayerState> EnumerateSprites();
    }
}