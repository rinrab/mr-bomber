// Copyright (c) Timofei Zhakov. All rights reserved.

using MrBoom.Common;
using MrBoom.Core.Sprites;
using MrBoom.Core.Sprites.Controllers;
using MrBoom.Core.Sprites.Interface;
using MrBoom.Core.Terrain;

namespace MrBoom.Core.Tests
{
    [TestFixture]
    public class SpriteTests
    {
        [Test]
        public void ServerPlayerTests()
        {
            var terrain = new MrBoom.Terrain(0, new SimpleRandom(34));

            var sprite = new ServerPlayer(0, 0, new ClientInfoFake());

            terrain.GetService<TerrainSpriteHost>().AddPlayer(sprite);

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

            var sprite = new ServerPlayer(0, 0, new ClientInfoFake());

            terrain.GetService<TerrainSpriteHost>().AddPlayer(sprite);

            var position = sprite.GetService<ISpritePositionProvider>();

            int x = position.X;
            int y = position.Y;

            sprite.GetService<SpriteMovementController>().Move(Directions.Up);

            Assert.AreEqual(x, position.X);
            Assert.AreEqual(y - 1, position.Y);
        }

        [Test]
        public void PlayerControllerTest()
        {
            var terrain = new MrBoom.Terrain(0, new SimpleRandom(34));

            var sprite = new ServerPlayer(0, 0, new ClientInfoFake());

            terrain.GetService<TerrainSpriteHost>().AddPlayer(sprite);

            var position = sprite.GetService<ISpritePositionProvider>();

            int x = position.X;
            int y = position.Y;

            sprite.GetService<SpriteMovementController>().SetDirection(null);
            sprite.ServerUpdate();
            Assert.AreEqual(x, position.X);
            Assert.AreEqual(y, position.Y);

            sprite.GetService<SpriteMovementController>().SetDirection(Directions.Up);
            sprite.ServerUpdate();
            Assert.AreEqual(x, position.X);
            Assert.AreEqual(y - 1, position.Y);

            sprite.GetService<SpriteMovementController>().SetDirection(null);
            sprite.ServerUpdate();
            Assert.AreEqual(x, position.X);
            Assert.AreEqual(y - 1, position.Y);
        }

        [Test]
        public void MonsterControllerTest()
        {
            var terrain = new MrBoom.Terrain(0, new SimpleRandom(34));

            var sprite = new ServerPlayer(0, 0, new ClientInfoFake());

            terrain.GetService<TerrainSpriteHost>().AddPlayer(sprite);

            var position = sprite.GetService<ISpritePositionProvider>();

            int x = position.X;
            int y = position.Y;

            sprite.GetService<SpriteMovementController>().SetDirection(null);
            sprite.ServerUpdate();
            Assert.AreEqual(x, position.X);
            Assert.AreEqual(y, position.Y);

            sprite.GetService<SpriteMovementController>().SetDirection(Directions.Up);
            sprite.ServerUpdate();
            Assert.AreEqual(x, position.X);
            Assert.AreEqual(y - 1, position.Y);

            sprite.GetService<SpriteMovementController>().SetDirection(null);
            sprite.ServerUpdate();
            Assert.AreEqual(x, position.X);
            Assert.AreEqual(y - 1, position.Y);
        }
    }
}
