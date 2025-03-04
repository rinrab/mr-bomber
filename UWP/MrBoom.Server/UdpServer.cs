// Copyright (c) Timofei Zhakov. All rights reserved.

using System.Net.Sockets;
using System.Net;
using MrBoom.NetworkProtocol.Messages;
using Microsoft.Extensions.Options;

namespace MrBoom.Server
{
    public delegate void PacketReceivedDelegate(Packet packet, IPEndPoint endPoint);

    public interface IUdpServer
    {
        Task SendPacket(Packet packet, IPEndPoint endPoint, CancellationToken cancellationToken);
        Task SendMessage(byte[] msg, IPEndPoint endPoint, CancellationToken cancellationToken);

        event PacketReceivedDelegate OnPacketReceived;
    }

    public class UdpServer : BackgroundService, IUdpServer
    {
        private readonly ILogger<UdpServer> logger;

        // TOOD: Add configuration.
        private readonly int port = 5297;
        private UdpClient udpClient;
        private IMetrics metrics;

        public UdpServer(ILogger<UdpServer> logger, IMetrics metrics, IOptions<Settings> options)
        {
            this.logger = logger;
            this.metrics = metrics;
            port = options.Value.LobbyPort;

            udpClient = new UdpClient(port);
        }

        public event PacketReceivedDelegate OnPacketReceived;

        public async Task SendMessage(byte[] msg, IPEndPoint endPoint, CancellationToken cancellationToken)
        {
            await udpClient.SendAsync(msg, endPoint, cancellationToken);
        }

        public async Task SendPacket(Packet packet, IPEndPoint endPoint, CancellationToken cancellationToken)
        {
            using var stream = new MemoryStream();
            using var writer = new BinaryWriter(stream);

            packet.WriteTo(writer);

            byte[] buffer = stream.GetBuffer();

            metrics.PacketSent(endPoint, buffer.Length);

            await SendMessage(buffer, endPoint, cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("UdpServer is starting");

            try
            {
                logger.LogInformation("Udp server binded to port {ListenPort}", port);

                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        UdpReceiveResult msg = await udpClient.ReceiveAsync(stoppingToken);

                        metrics.PacketReceived(msg.RemoteEndPoint, msg.Buffer.Length);

                        using Stream stream = new MemoryStream(msg.Buffer);
                        using BinaryReader reader = new BinaryReader(stream);

                        try
                        {
                            var packet = new Packet();
                            packet.ReadFrom(reader);

                            try
                            {
                                OnPacketReceived?.Invoke(packet, msg.RemoteEndPoint);
                                logger.LogDebug("Received packet {Packet} from {RemoteEndPoint}", packet, msg.RemoteEndPoint);
                            }
                            catch (Exception ex)
                            {
                                logger.LogError(ex, "Failed to process message from {RemoteEndPoint}", msg.RemoteEndPoint);
                            }
                        }
                        catch (Exception ex)
                        {
                            logger.LogError(ex, "Failed to read message from {RemoteEndPoint}", msg.RemoteEndPoint);
                            continue;
                        }
                    }
                    catch (SocketException ex)
                    {
                        if (ex.SocketErrorCode == SocketError.ConnectionReset)
                        {
                            // Ignore connection reset errors.
                            continue;
                        }
                        else
                        {
                            throw;
                        }
                    }
                }
            }
            catch (Exception)
            {
                if (!stoppingToken.IsCancellationRequested)
                {
                    throw;
                }
            }

            logger.LogInformation("UdpServer stopped");
        }
    }
}
