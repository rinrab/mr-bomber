// Copyright (c) Timofei Zhakov. All rights reserved.


using MrBoom.Common;

namespace MrBoom.Server
{
    public class LobbyPlayer
    {
        public string Name { get; }
        public Guid Id { get; set; }
        public int Index { get; set; }

        public LobbyPlayer(string name)
        {
            Name = name;
        }
    }
}
