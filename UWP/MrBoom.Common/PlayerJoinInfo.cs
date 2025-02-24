// Copyright (c) Timofei Zhakov. All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace MrBoom.Common
{
    public class PlayerJoinInfo
    {
        [JsonRequired]
        [JsonPropertyName("name")]
        public string Name { get; set; }
    }
}
