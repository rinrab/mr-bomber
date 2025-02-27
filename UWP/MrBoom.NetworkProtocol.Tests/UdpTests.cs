// Copyright (c) Timofei Zhakov. All rights reserved.

using System.Net.Sockets;
using MrBoom.NetworkProtocol.Messages;

namespace MrBoom.NetworkProtocol.Tests
{
    public class UdpTests
    {
        [Test]
        public async Task JoinRequester()
        {
            using (var stream = new MemoryStream())
            {
                var msg = new PlayerJoin
                {
                    Name = "yemete kudasai"
                };
                var packet = new Packet(msg);
                packet.WriteTo(new BinaryWriter(stream));

                var client = new UdpClient(0);
                client.Connect("master._mrboomserver.test.mrbomber.online", 5297);

                await client.SendAsync(stream.GetBuffer(), (int)stream.Length);
            }
        }
    }
}
