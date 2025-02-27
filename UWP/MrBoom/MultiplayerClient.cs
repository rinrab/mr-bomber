// Copyright (c) Timofei Zhakov. All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using MrBoom.Common;
using MrBoom.NetworkProtocol;
using MrBoom.NetworkProtocol.Messages;

namespace MrBoom
{
    public delegate void PacketReceivedDelegate(Packet packet);

    public class MultiplayerClient
    {
        private HttpClient client;
        private UdpClient udpClient;

        private Queue<byte[]> packetQueue = new Queue<byte[]>();

        // public Uri MasterServerUri = new Uri("http://master._mrboomserver.test.mrbomber.online:5296");
        public Uri MasterServerUri = new Uri("http://localhost:5296");

        public event PacketReceivedDelegate OnPacketReceived;

        public MultiplayerClient()
        {
            client = new HttpClient();
            udpClient = new UdpClient();
        }

        public void CheckPackets()
        {
            while (packetQueue.TryDequeue(out var bytes))
            {
                using (Stream stream = new MemoryStream(bytes))
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    var packet = new Packet();
                    packet.ReadFrom(reader);

                    OnPacketReceived?.Invoke(packet);
                }
            }
        }

        public async Task ListenAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                UdpReceiveResult msg = await udpClient.ReceiveAsync();
                packetQueue.Enqueue(msg.Buffer);
            }
        }

        public async Task ConnectLobby(ClientJoinResponse lobby)
        {
            udpClient.Connect(lobby.LobbyIp, lobby.LobbyPort);

            var msg = new Packet(new ClientJoin
            {
                ClientSecret = lobby.ClientSecret,
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

        public async Task SendPacket(Packet packet)
        {
            using (var stream = new MemoryStream())
            using (var writer = new BinaryWriter(stream))
            {
                packet.WriteTo(writer);
                await udpClient.SendAsync(stream.GetBuffer(), (int)stream.Length);
            }
        }
    }
}
