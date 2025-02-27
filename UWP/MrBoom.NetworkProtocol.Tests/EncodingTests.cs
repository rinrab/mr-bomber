// Copyright (c) Timofei Zhakov. All rights reserved.

using MrBoom.NetworkProtocol.Messages;

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
                    Id = new Guid("BE596A86-74E1-4E9B-82E7-94368AAC39C7")
                };
                var packet = new Packet(msg);
                packet.WriteTo(new BinaryWriter(stream));

                stream.Seek(0, SeekOrigin.Begin);

                var packet2 = new Packet();
                packet2.ReadFrom(new BinaryReader(stream));

                Assert.AreEqual(msg.Id, ((PlayerJoin)packet2.Message).Id);
            }
        }
    }
}
