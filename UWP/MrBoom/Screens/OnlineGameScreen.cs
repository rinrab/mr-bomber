// Copyright (c) Timofei Zhakov. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using MrBoom.Core.Terrain;
using MrBoom.NetworkProtocol.Messages;
using MrBoom.NetworkProtocol.Proxy;
using MrBoom.State;

namespace MrBoom.Screens
{
    public class PlayerProvider
    {
        public int MaxPlayers { get; } = 8;

        private readonly List<IPlayerState> players;
        private readonly List<IPlayerState> monsters;

        public int Count => players.Count;
        public IPlayerState this[int index] => players[index];

        public PlayerProvider()
        {
            players = new List<IPlayerState>();
            monsters = new List<IPlayerState>();

            InitializeMonsters();
        }

        public PlayerProvider(List<IPlayerState> players)
        {
            this.players = players;
            monsters = new List<IPlayerState>();

            InitializeMonsters();
        }

        private void InitializeMonsters()
        {
            for (int i = 0; i < MaxPlayers - players.Count; i++)
            {
                monsters.Add(new OnlineMonsterPlayerState(i));
            }
        }

        public bool AddPlayer(Func<int, IPlayerState> providePlayer)
        {
            if (players.Count < MaxPlayers)
            {
                players.Add(providePlayer(players.Count));
                return true;
            }
            else
            {
                for (int i = 0; i < players.Count; i++)
                {
                    if (players[i].IsReplaceble)
                    {
                        players[i] = providePlayer(i);

                        return true;
                    }
                }

                return false;
            }
        }

        public void Clear()
        {
            players.Clear();
        }

        public IEnumerable<IPlayerState> EnumeratePlayers()
        {
            int count = 0;

            for (int i = 0; i < players.Count && count < MaxPlayers; i++, count++)
            {
                yield return players[i];
            }
        }

        public IEnumerable<IPlayerState> EnumerateSprites()
        {
            int count = 0;

            for (int i = 0; i < players.Count && count < MaxPlayers; i++, count++)
            {
                yield return players[i];
            }

            for (int i = 0; i < monsters.Count && count < MaxPlayers; i++, count++)
            {
                yield return monsters[i];
            }
        }
    }

    public class OnlineGameScreen : ClientGameScreen
    {
        private readonly MultiplayerClient multiplayerClient;
        private readonly TerrainProxy terrainProxy;

        private readonly PlayerProvider players;

        public OnlineGameScreen(Assets assets, MultiplayerClient multiplayerClient, PlayerProvider players) : base(assets)
        {
            terrainProxy = new TerrainProxy();

            this.multiplayerClient = multiplayerClient;
            this.players = players;

            multiplayerClient.OnPacketReceived += OnPacketReceived;
        }

        private void OnPacketReceived(Packet packet)
        {
            if (packet.Message is GameInfo gameInfo)
            {
                bool init = false;

                if (clientTerrain == null)
                {
                    init = true;

                    clientTerrain = new ClientTerrain(terrainProxy, assets);

                    foreach (IPlayerState state in players.EnumerateSprites())
                    {
                        terrainProxy.Sprites.Add(state.InitializeProxy());
                    }
                }

                terrainProxy.SetIncomingMessage(gameInfo);

                if (init)
                {
                    foreach (IPlayerState state in players.EnumerateSprites())
                    {
                        clientTerrain.Sprites.Add(state.InitializeClientSprite(terrainProxy, assets));
                    }
                }
            }
        }

        public override void Update()
        {
            multiplayerClient.CheckPackets();

            if (clientTerrain != null)
            {
                terrainProxy.ClientUpdate();

                _ = multiplayerClient.SendPacket(new Packet(terrainProxy.GetOutcomingMessage())
                {
                    Lobby = multiplayerClient.LobbyId,
                    ClientSecret = multiplayerClient.ClientSecret,
                });

                base.Update();
            }

            if (multiplayerClient.IsDead())
            {
                ScreenManager.SetScreen(new OnlineConnectionDiedScreen(assets));
            }
        }

        public override void Draw(SpriteBatch ctx)
        {
            if (clientTerrain != null)
            {
                base.Draw(ctx);
            }
        }
    }
}
