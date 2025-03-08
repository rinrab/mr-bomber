// Copyright (c) Timofei Zhakov. All rights reserved.

using System;
using MrBoom.Core.Terrain;
using MrBoom.NetworkProtocol.Messages;
using MrBoom.NetworkProtocol.Proxy;
using MrBoom.Screens;

namespace MrBoom.State
{
    public class OnlineRemotePlayerState : IPlayerState
    {
        private LobbyPlayerInfo info;

        public int Index { get => info.Index; }
        public string Name { get => info.Name; }
        public int VictoryCount { get; set; }
        public bool IsReplaceble => false;

        private ISpriteProxy proxy;

        public OnlineRemotePlayerState(LobbyPlayerInfo info)
        {
            this.info = info;
        }

        public ServerPlayer InitializeServerPlayer(Terrain terrain, int team)
        {
            throw new NotImplementedException();
        }

        public ISpriteProxy InitializeProxy(IExtensibilityProvider extensibility)
        {
            proxy = new SpriteProxy();
            proxy = extensibility.WrapSpriteProxy(proxy);
            return proxy;
        }

        public IClientSprite InitializeClientSprite(ITerrainProxy terrain, Assets assets)
        {
            return new ClientSprite(proxy, assets);
        }
    }
}
