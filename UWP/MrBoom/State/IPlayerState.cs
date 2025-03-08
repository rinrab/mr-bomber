// Copyright (c) Timofei Zhakov. All rights reserved.

using MrBoom.Core.Terrain;
using MrBoom.Screens;

namespace MrBoom.State
{
    public interface IPlayerState
    {
        int Index { get; }
        string Name { get; }
        int VictoryCount { get; set; }
        bool IsReplaceble { get; }

        ServerPlayer InitializeServerPlayer(Terrain terrain, int team);
        ISpriteProxy InitializeProxy(IExtensibilityProvider extensibility);
        IClientSprite InitializeClientSprite(ITerrainProxy terrain, Assets assets);
    }
}
