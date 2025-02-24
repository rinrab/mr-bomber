// Copyright (c) Timofei Zhakov. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using MrBoom.Common;

namespace MrBoom
{
    public class MultiplayerClient
    {
        private HttpClient client;

        public MultiplayerClient()
        {
            client = new HttpClient();
        }

        public async Task<PlayerInfo> Join(PlayerJoinInfo player)
        {
            HttpContent content = new StringContent(
                JsonSerializer.Serialize(player),
                Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync(
                new Uri("http://localhost:5296/api/v1/game"), content);

            return JsonSerializer.Deserialize<PlayerInfo>(
                await response.Content.ReadAsStringAsync());
        }
    }
}
