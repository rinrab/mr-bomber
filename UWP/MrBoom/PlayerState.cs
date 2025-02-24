// Copyright (c) Timofei Zhakov. All rights reserved.

using System;
using MrBoom.Bot;
using MrBoom.Common;

namespace MrBoom
{
    public interface IPlayerState
    {
        int Index { get; }
        string Name { get; }
        int VictoryCount { get; set; }
        bool IsReplaceble { get; }

        AbstractPlayer GetPlayer(Terrain terrain, int team);
    }

    public interface IOnlinePlayerState : IPlayerState
    {
        Guid Id { get; }
    }

    public class HumanPlayerState : IPlayerState
    {
        public IController Controller { get; }
        public int Index { get; }
        public string Name { get; }
        public int VictoryCount { get; set; }
        public bool IsReplaceble => false;

        public HumanPlayerState(IController controller, int index, string name)
        {
            Controller = controller;
            Index = index;
            Name = name;
        }

        public AbstractPlayer GetPlayer(Terrain terrain, int team)
        {
            return new Human(terrain, terrain.assets.Players[Index], Controller, team);
        }
    }

    public class OnlinePlayerState : IOnlinePlayerState
    {
        private Guid _id = Guid.Empty;

        public IController Controller { get; }
        public int Index { get; private set; }
        private string _name;
        public int VictoryCount { get; set; }
        public bool IsReplaceble => false;

        private bool isLoaded = false;

        public string Name
        {
            get
            {
                if (!isLoaded)
                {
                    return "...";
                }
                else
                {
                    return _name;
                }
            }
        }

        public Guid Id { get => _id; }

        public OnlinePlayerState(IController controller, string name)
        {
            Controller = controller;
            Index = -1;
            _name = name;
        }

        public void OnLoaded(PlayerInfo info)
        {
            _id = info.Id;
            Index = info.Index;
            isLoaded = true;
        }

        public AbstractPlayer GetPlayer(Terrain terrain, int team)
        {
            throw new NotImplementedException();
        }
    }

    public class OnlineRemotePlayerState : IPlayerState
    {
        private PlayerInfo info;

        public int Index { get => info.Index; }
        public string Name { get => info.Name; }
        public int VictoryCount { get; set; }
        public bool IsReplaceble => false;

        public OnlineRemotePlayerState(PlayerInfo info)
        {
            this.info = info;
        }

        public AbstractPlayer GetPlayer(Terrain terrain, int team)
        {
            throw new NotImplementedException();
        }
    }

    public class BotPlayerState : IPlayerState
    {
        public int Index { get; }
        public string Name { get; }
        public int VictoryCount { get; set; }
        public bool IsReplaceble => true;

        public BotPlayerState(int index, string name)
        {
            Index = index;
            Name = name;
        }

        public AbstractPlayer GetPlayer(Terrain terrain, int team)
        {
            return new ComputerPlayer(terrain, terrain.assets.Players[Index], team, Index);
        }
    }
}
