// Copyright (c) Timofei Zhakov. All rights reserved.

namespace MrBoom.Core.Sprites.Interface
{
    public interface IBombOwner
    {
        bool IsAllowed { get; }
        bool RemoteDetonate { get; }

        void OnBombReplaced(int x, int y);
    }
}
