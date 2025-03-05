// Copyright (c) Timofei Zhakov. All rights reserved.

using System.Collections.Generic;

namespace MrBoom.State
{
    public interface IPlayerProvider : IReadOnlyList<IPlayerState>
    {
        IEnumerable<IPlayerState> EnumerateSprites();
    }
}
