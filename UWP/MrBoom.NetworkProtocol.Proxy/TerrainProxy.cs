// Copyright (c) Timofei Zhakov. All rights reserved.

using System.Collections.Generic;
using System.Linq;
using MrBoom.Core.Terrain;
using MrBoom.NetworkProtocol.Messages;

namespace MrBoom.NetworkProtocol.Proxy
{
    public class TerrainProxy : ITerrainProxy, IRemoteProxy
    {
        private GameInfo message;

        public int TimeLeft { get; set; }
        public int ApocalypseSpeed { get; set; }
        public int MaxApocalypse { get; set; }
        public int Width => message.Terrain.Width;
        public int Height => message.Terrain.Width;
        public int LevelIndex => message.LevelIndex;

        private IList<SpriteProxy> _sprites;
        public IList<ISpriteProxy> Sprites
        {
            get
            {


                return _sprites.Cast<ISpriteProxy>().ToList();
            }
        }

        public TerrainProxy()
        {
        }

        public void ClientUpdate()
        {
        }

        public Cell GetCell(int x, int y)
        {
            var cell = message.Terrain.Grid[x, y];

            if (cell == null)
            {
                return new Cell(TerrainType.PermanentWall);
            }
            else
            {
                return new Cell(cell.Type)
                {
                };
            }
        }

        public bool IsWalkable(int x, int y)
        {
            Cell cell = GetCell(x, y);

            switch (cell.Type)
            {
                case TerrainType.Free:
                case TerrainType.PowerUpFire:
                    return true;

                case TerrainType.PermanentWall:
                case TerrainType.Rubber:
                case TerrainType.Apocalypse:
                    return false;

                case TerrainType.TemporaryWall:
                case TerrainType.Bomb:
                    return false; // cheats.noClip

                default:
                    return true;
            }
        }

        public void SetIncomingMessage(IMessage message)
        {
            this.message = (GameInfo)message;

            if (_sprites == null)
            {
                _sprites = new List<SpriteProxy>(this.message.Sprites.Count);
                int i = 0;

                foreach (var submessage in this.message.Sprites)
                {
                    SpriteProxy sprite;

                    if (submessage.Type == GameSpriteType.Player)
                    {
                        sprite = new PlayerProxy(i);
                    }
                    else
                    {
                        sprite = new SpriteProxy();
                    }

                    sprite.SetIncomingMessage(submessage);

                    _sprites.Add(sprite);
                    i++;
                }
            }
            else
            {
                for (int i = 0; i < this.message.Sprites.Count && i < _sprites.Count; i++)
                {
                    _sprites[i].SetIncomingMessage(this.message.Sprites[i]);
                }
            }
        }

        public IMessage GetOutcomingMessage()
        {
            var sprites = new List<ClientPlayerUpdateMessage>();

            foreach (var sprite in _sprites)
            {
                var msg = sprite.GetOutcomingMessage();

                if (msg != null)
                {
                    sprites.Add((ClientPlayerUpdateMessage)msg);
                }
            }

            return new ClientUpdateMessage
            {
                SpriteUpdates = sprites,
            };
        }
    }
}
