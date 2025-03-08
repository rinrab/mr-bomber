// Copyright (c) Timofei Zhakov. All rights reserved.

using MrBoom.Core;
using MrBoom.Core.Terrain;
using MrBoom.Screens;

namespace MrBoom.State
{
    public class SinglePlayerHumanPlayerState : IPlayerState
    {
        public IController Controller { get; }
        public int Index { get; }
        public string Name { get; }
        public int VictoryCount { get; set; }
        public bool IsReplaceble => false;

        private ServerPlayer proxy;

        public SinglePlayerHumanPlayerState(IController controller, int index, string name)
        {
            Controller = controller;
            Index = index;
            Name = name;
        }

        public ServerPlayer InitializeServerPlayer(Terrain terrain, int team)
        {
            proxy = new ServerPlayer(terrain, team, Index, new ClientInfoFake());
            return proxy;
        }

        public ISpriteProxy InitializeProxy(IExtensibilityProvider extensibility)
        {
            return proxy;
        }

        public IClientSprite InitializeClientSprite(ITerrainAccessor terrain, Assets assets)
        {
            return new ClientSpriteLocalHuman(terrain, proxy, assets, Controller);
        }
    }
}
