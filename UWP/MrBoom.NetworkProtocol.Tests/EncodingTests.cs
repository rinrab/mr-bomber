// Copyright (c) Timofei Zhakov. All rights reserved.

using MrBoom.Common;
using MrBoom.Common.Messages;

namespace MrBoom.NetworkProtocol.Tests
{
    public class EncodingTests
    {
        [Test]
        public void PlayerJoinPacketMessage()
        {
            using (var stream = new MemoryStream())
            {
                var msg = new PlayerJoin
                {
                    Name = "sigma"
                };
                var packet = new Packet(msg);
                packet.WriteTo(new BinaryWriter(stream));

                stream.Seek(0, SeekOrigin.Begin);

                var packet2 = new Packet();
                packet2.ReadFrom(new BinaryReader(stream));

                Assert.AreEqual(msg.Name, ((PlayerJoin)packet2.Message).Name);
            }
        }
    }
}
