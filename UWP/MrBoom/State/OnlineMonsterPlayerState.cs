// Copyright (c) Timofei Zhakov. All rights reserved.

using System;
using MrBoom.Core;
using MrBoom.Core.Terrain;
using MrBoom.NetworkProtocol.Proxy;

namespace MrBoom.State
{
    public class OnlineMonsterPlayerState : IPlayerState
    {
        public int Index { get; }
        public string Name { get => throw new NotImplementedException(); }
        public int VictoryCount { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool IsReplaceble => true;

        private ISpriteProxy proxy;

        public OnlineMonsterPlayerState(int index)
        {
            Index = index;
        }

        public ServerPlayer InitializeServerPlayer(Terrain terrain, int team)
        {
            throw new NotImplementedException();
        }

        public ISpriteProxy InitializeProxy()
        {
            proxy = new SpriteProxy();
            return proxy;
        }

        public IClientSprite InitializeClientSprite(ITerrainAccessor terrain, Assets assets)
        {
            return new ClientSprite(proxy, assets);
        }
    }
}
