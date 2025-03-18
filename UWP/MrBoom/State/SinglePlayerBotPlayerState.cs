// Copyright (c) Timofei Zhakov. All rights reserved.

using MrBoom.Bot;
using MrBoom.Core.Sprites;
using MrBoom.Core.Terrain;
using MrBoom.Screens;

namespace MrBoom.State
{
    public class SinglePlayerBotPlayerState : IPlayerState
    {
        public int Index { get; }
        public string Name { get; }
        public int VictoryCount { get; set; }
        public bool IsReplaceble => true;

        private ServerPlayer proxy;

        public SinglePlayerBotPlayerState(int index, string name)
        {
            Index = index;
            Name = name;
        }

        public ServerPlayer InitializeServerPlayer(Terrain terrain, int team)
        {
            proxy = new ComputerPlayer(terrain, team, Index, Index);
            return proxy;
        }

        public ISpriteProxy InitializeProxy(IExtensibilityProvider extensibility)
        {
            throw new System.NotImplementedException();
        }

        public GameEntityBase InitializeClientSprite(ITerrainProxy terrain, Assets assets)
        {
            return new ClientSprite(proxy.GetService<ISpriteProxy>(), assets);
        }
    }
}
