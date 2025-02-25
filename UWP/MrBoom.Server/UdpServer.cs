// Copyright (c) Timofei Zhakov. All rights reserved.

using System.Net.Sockets;
using System.Net;

namespace MrBoom.Server
{
    public delegate void MessageReceivedDelegate(UdpReceiveResult msg);

    public interface IUdpServer
    {
        Task SendMessage(byte[] msg, IPEndPoint endPoint, CancellationToken cancellationToken);

        event MessageReceivedDelegate OnMesssageReceived;
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

        public event MessageReceivedDelegate OnMesssageReceived;

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
                    UdpReceiveResult msg;
                    try
                    {
                        msg = await udpClient.ReceiveAsync(stoppingToken);
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

                    logger.LogDebug("Received message from {RemoteEndPoint}", msg.RemoteEndPoint);

                    OnMesssageReceived?.Invoke(msg);
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
