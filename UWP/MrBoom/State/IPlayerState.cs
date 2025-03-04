// Copyright (c) Timofei Zhakov. All rights reserved.

using MrBoom.Core.Terrain;

namespace MrBoom.State
{
    public interface IPlayerState
    {
        int Index { get; }
        string Name { get; }
        int VictoryCount { get; set; }
        bool IsReplaceble { get; }

        ServerPlayer InitializeServerPlayer(Terrain terrain, int team);
        ISpriteProxy InitializeProxy();
        IClientSprite InitializeClientSprite(ITerrainAccessor terrain, Assets assets);
    }
}
