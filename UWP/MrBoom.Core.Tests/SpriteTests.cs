// Copyright (c) Timofei Zhakov. All rights reserved.

using MrBoom.Common;
using MrBoom.Core.Sprites;

namespace MrBoom.Core.Tests
{
    [TestFixture]
    public class SpriteTests
    {
        [Test]
        public void ServerPlayerTests()
        {
            var terrain = new MrBoom.Terrain(0, new SimpleRandom(34));
            var sprite = new ServerPlayer(terrain, 0, 0, new ClientInfoFake());

            terrain.sprites.AddPlayer(sprite);

            var position = sprite.GetService<ISpritePositionProvider>();

            int x = position.X;
            int y = position.Y;

            sprite.ServerUpdate();
            sprite.ServerUpdate();
            sprite.ServerUpdate();
            sprite.ServerUpdate();

            Assert.AreEqual(x, position.X);
            Assert.AreEqual(y, position.Y);
        }

        [Test]
        public void SpriteMovementControllerTests()
        {
            var terrain = new MrBoom.Terrain(0, new SimpleRandom(34));
            var sprite = new ServerPlayer(terrain, 0, 0, new ClientInfoFake());

            terrain.sprites.AddPlayer(sprite);

            var position = sprite.GetService<ISpritePositionProvider>();

            int x = position.X;
            int y = position.Y;

            sprite.GetService<SpriteMovementController>().Move(Directions.Down);

            Assert.AreEqual(x, position.X);
            Assert.AreEqual(y + 1, position.Y);
        }

        [Test]
        public void PlayerControllerTest()
        {
            var terrain = new MrBoom.Terrain(0, new SimpleRandom(34));

            var sprite = new ServerPlayer(terrain, 0, 0, new ClientInfoFake());
            var position = sprite.GetService<ISpritePositionProvider>();
            sprite.AddSingleton<PlayerController>();

            terrain.sprites.AddPlayer(sprite);

            int x = position.X;
            int y = position.Y;

            sprite.GetService<PlayerController>().SetDirection(null);
            sprite.ServerUpdate();
            Assert.AreEqual(x, position.X);
            Assert.AreEqual(y, position.Y);

            sprite.GetService<PlayerController>().SetDirection(Directions.Down);
            sprite.ServerUpdate();
            Assert.AreEqual(x, position.X);
            Assert.AreEqual(y + 1, position.Y);

            sprite.GetService<PlayerController>().SetDirection(null);
            sprite.ServerUpdate();
            Assert.AreEqual(x, position.X);
            Assert.AreEqual(y + 1, position.Y);
        }

        [Test]
        public void MonsterControllerTest()
        {
            var terrain = new MrBoom.Terrain(0, new SimpleRandom(34));

            var sprite = new ServerPlayer(terrain, 0, 0, new ClientInfoFake());
            var position = sprite.GetService<ISpritePositionProvider>();
            sprite.AddSingleton<MonsterController>();

            terrain.sprites.AddPlayer(sprite);

            int x = position.X;
            int y = position.Y;

            sprite.GetService<MonsterController>().SetDirection(null);
            sprite.ServerUpdate();
            Assert.AreEqual(x, position.X);
            Assert.AreEqual(y, position.Y);

            sprite.GetService<MonsterController>().SetDirection(Directions.Down);
            sprite.ServerUpdate();
            Assert.AreEqual(x, position.X);
            Assert.AreEqual(y + 1, position.Y);

            sprite.GetService<MonsterController>().SetDirection(null);
            sprite.ServerUpdate();
            Assert.AreEqual(x, position.X);
            Assert.AreEqual(y + 1, position.Y);
        }
    }
}
