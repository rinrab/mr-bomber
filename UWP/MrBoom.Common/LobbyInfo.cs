// Copyright (c) Timofei Zhakov. All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace MrBoom.Common
{
    public class LobbyInfo
    {
        [JsonRequired]
        [JsonPropertyName("players")]
        public List<PlayerInfo> Players { get; set; }
    }
}
