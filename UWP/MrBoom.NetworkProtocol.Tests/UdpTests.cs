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
                    Id = new Guid("00000000-0000-0000-0000-000000000011"),
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
