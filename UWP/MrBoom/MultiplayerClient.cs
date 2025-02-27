// Copyright (c) Timofei Zhakov. All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using MrBoom.Common;
using MrBoom.Common.Messages;
using MrBoom.NetworkProtocol;

namespace MrBoom
{
    public class MultiplayerClient
    {
        private HttpClient client;
        private UdpClient udpClient;

        // public Uri MasterServerUri = new Uri("http://master._mrboomserver.test.mrbomber.online:5296");
        public Uri MasterServerUri = new Uri("http://localhost:5296");

        public MultiplayerClient()
        {
            client = new HttpClient();
            udpClient = new UdpClient();
        }

        public async Task ConnectLobby(ClientJoinResponse lobby)
        {
            udpClient.Connect(lobby.LobbyIp, lobby.LobbyPort);

            var msg = new Packet(new PlayerJoin
            {
                Name = "sigma boy"
            });

            using (var stream = new MemoryStream())
            {
                msg.WriteTo(new BinaryWriter(stream));
                await udpClient.SendAsync(stream.GetBuffer(), (int)stream.Length);
            }
        }

        public async Task<ClientJoinResponse> JoinLobby(ClientJoinRequest request)
        {
            HttpContent content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync(new Uri(MasterServerUri, "api/v1/master/join"), content);
            return JsonSerializer.Deserialize<ClientJoinResponse>(await response.Content.ReadAsStringAsync());
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

        public async Task<LobbyInfo> GetLobby()
        {
            HttpResponseMessage response = await client.GetAsync(
                new Uri("http://localhost:5296/api/v1/game"));

            return JsonSerializer.Deserialize<LobbyInfo>(
                await response.Content.ReadAsStringAsync());
        }
    }
}
