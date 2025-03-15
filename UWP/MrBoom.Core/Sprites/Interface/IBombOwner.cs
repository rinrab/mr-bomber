// Copyright (c) Timofei Zhakov. All rights reserved.

namespace MrBoom.Core.Sprites
{
    public interface IBombOwner
    {
        bool IsAllowed { get; }
        bool RemoteDetonate { get; }

        void OnBombReplaced(int x, int y);
    }
}
