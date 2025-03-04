// Copyright (c) Timofei Zhakov. All rights reserved.

using MrBoom.Bot;

namespace MrBoom.State
{
    public class SinglePlayerBotPlayerState : IPlayerState
    {
        public int Index { get; }
        public string Name { get; }
        public int VictoryCount { get; set; }
        public bool IsReplaceble => true;

        public SinglePlayerBotPlayerState(int index, string name)
        {
            Index = index;
            Name = name;
        }

        public ServerPlayer GetPlayer(Terrain terrain, int team)
        {
            return new ComputerPlayer(terrain, team, Index, Index);
        }
    }
}
