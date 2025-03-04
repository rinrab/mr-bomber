// Copyright (c) Timofei Zhakov. All rights reserved.

namespace MrBoom.State
{
    public interface IPlayerState
    {
        int Index { get; }
        string Name { get; }
        int VictoryCount { get; set; }
        bool IsReplaceble { get; }

        ServerPlayer GetPlayer(Terrain terrain, int team);
    }
}
