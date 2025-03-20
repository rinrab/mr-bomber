// Copyright (c) Timofei Zhakov. All rights reserved.

namespace MrBoom.Server
{
    public class Settings
    {
        public const string Name = "Settings";

        public int LobbyPort { get; set; } = 5050;
        public string LobbyIp { get; set; } = "localhost";

        public string MasterUrl { get; set; } = "http://localhost:5050";
    }
}
