// Copyright (c) Timofei Zhakov. All rights reserved.

using System;
using System.Text.Json.Serialization;

namespace MrBoom.NetworkProtocol
{
    public class ClientJoinResponse
    {
        [JsonRequired]
        [JsonPropertyName("lobby_ip")]
        public string LobbyIp { get; set; }

        [JsonRequired]
        [JsonPropertyName("lobby_port")]
        public int LobbyPort { get; set; }

        [JsonRequired]
        [JsonPropertyName("client_secret")]
        public Guid ClientSecret { get; set; }
    }
}
