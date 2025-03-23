// Copyright (c) Timofei Zhakov. All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
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

        private DateTime? lastPacketReceived;

        // public Uri MasterServerUri = new Uri("http://master._mrboomserver.test.mrbomber.online:5296");
        public Uri MasterServerUri = new Uri("http://localhost:5050");
        // public Uri MasterServerUri = new Uri("http://eu.mrbomber.online");

        public Guid ClientSecret { get; private set; }

        public Guid LobbyId { get; private set; }

        public event PacketReceivedDelegate OnPacketReceived;

        public MultiplayerClient()
        {
            client = new HttpClient();
            udpClient = new UdpClient();
            lastPacketReceived = null;
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

                    lastPacketReceived = DateTime.UtcNow;
                }
            }
        }

        public bool IsDead()
        {
            if (lastPacketReceived == null)
            {
                return false;
            }
            else
            {
                return DateTime.UtcNow - lastPacketReceived > TimeSpan.FromMilliseconds(1000);
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

            var msg = new Packet()
            {
                Lobby = LobbyId,
                ClientSecret = ClientSecret,
                Message = new ClientJoin
                {
                },
            };

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
            ClientJoinResponse res = JsonSerializer.Deserialize<ClientJoinResponse>(await response.Content.ReadAsStringAsync());

            ClientSecret = res.ClientSecret;
            LobbyId = res.LobbyId;

            return res;
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
