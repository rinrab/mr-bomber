// Copyright (c) Timofei Zhakov. All rights reserved.

using System.Net.Sockets;
using System.Net;
using MrBoom.NetworkProtocol.Messages;

namespace MrBoom.Server
{
    public delegate void PacketReceivedDelegate(Packet packet);

    public interface IUdpServer
    {
        Task SendMessage(byte[] msg, IPEndPoint endPoint, CancellationToken cancellationToken);

        event PacketReceivedDelegate OnPacketReceived;
    }

    public class UdpServer : BackgroundService, IUdpServer
    {
        private readonly ILogger<UdpServer> logger;

        // TOOD: Add configuration.
        private readonly int port = 5297;
        private UdpClient udpClient;

        public UdpServer(ILogger<UdpServer> logger)
        {
            this.logger = logger;
            udpClient = new UdpClient(port);
        }

        public event PacketReceivedDelegate OnPacketReceived;

        public async Task SendMessage(byte[] msg, IPEndPoint endPoint, CancellationToken cancellationToken)
        {
            await udpClient.SendAsync(msg, endPoint, cancellationToken);
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

                        using Stream stream = new MemoryStream(msg.Buffer);
                        using BinaryReader reader = new BinaryReader(stream);

                        try
                        {
                            var packet = new Packet();
                            packet.ReadFrom(reader);

                            try
                            {
                                OnPacketReceived?.Invoke(packet);
                                logger.LogInformation("Received packet {Packet} from {RemoteEndPoint}", packet, msg.RemoteEndPoint);
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
