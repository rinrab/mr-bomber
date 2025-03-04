// Copyright (c) Timofei Zhakov. All rights reserved.

using MrBoom.Core;

namespace MrBoom.State
{
    public class SinglePlayerHumanPlayerState : IPlayerState
    {
        public IController Controller { get; }
        public int Index { get; }
        public string Name { get; }
        public int VictoryCount { get; set; }
        public bool IsReplaceble => false;

        public SinglePlayerHumanPlayerState(IController controller, int index, string name)
        {
            Controller = controller;
            Index = index;
            Name = name;
        }

        public ServerPlayer GetPlayer(Terrain terrain, int team)
        {
            return new ServerPlayer(terrain, team, Index, new ClientInfoFake());
        }
    }
}
