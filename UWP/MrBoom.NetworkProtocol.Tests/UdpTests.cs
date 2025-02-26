// Copyright (c) Timofei Zhakov. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MrBoom.Common.Messages;
using MrBoom.Common;
using System.Net.Sockets;

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
                client.Connect("localhost", 5297);

                await client.SendAsync(stream.GetBuffer(), (int)stream.Length);
            }
        }
    }
}
