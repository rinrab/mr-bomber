// Copyright (c) Timofei Zhakov. All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MrBoom.NetworkProtocol.Messages;
using MrBoom.NetworkProtocol.Proxy;
using MrBoom.State;

namespace MrBoom.Screens
{
    public class IndexedPlayerProvider : IRemoteProxy, IPlayerProvider
    {
        private readonly IDictionary<Guid, IPlayerState> playerIndex;

        private LobbyPlayerCollection message;

        public int Count => EnumeratePlayers().Count();
        public IPlayerState this[int index] => EnumeratePlayers().ElementAt(index);

        public IndexedPlayerProvider()
        {
            playerIndex = new Dictionary<Guid, IPlayerState>();
        }

        public void AddPlayer(Guid id, IPlayerState player)
        {
            playerIndex.Add(id, player);
        }

        public IEnumerable<IPlayerState> EnumeratePlayers()
        {
            if (message != null)
            {
                foreach (LobbyPlayerInfo msg in message.Children)
                {
                    if (playerIndex.TryGetValue(msg.Key, out IPlayerState player))
                    {
                        if (player is OnlineLocalPlayerState localPlayer)
                        {
                            localPlayer.OnLoaded(msg);
                        }

                        yield return player;
                    }
                    else
                    {
                        OnlineRemotePlayerState newPlayer = new OnlineRemotePlayerState(msg);
                        playerIndex.Add(msg.Key, newPlayer);
                        yield return newPlayer;
                    }
                }
            }
        }

        public void Vacuum(PlayerProvider playerProvider)
        {
            playerProvider.Clear();

            foreach (IPlayerState player in EnumeratePlayers())
            {
                playerProvider.AddPlayer(_ => player);
            }
        }

        public void SetIncomingMessage(IMessage message)
        {
            this.message = (LobbyPlayerCollection)message;
        }

        public IMessage GetOutcomingMessage()
        {
            return null;
        }

        public IEnumerator<IPlayerState> GetEnumerator()
        {
            return EnumeratePlayers().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return EnumeratePlayers().GetEnumerator();
        }

        public IEnumerable<IPlayerState> EnumerateSprites()
        {
            return EnumeratePlayers();
        }
    }

    public class OnlineStartScreen : AbstractStartScreen
    {
        private readonly MultiplayerClient multiplayerClient;
        private int multiplayerStartIn = -1;

        private readonly IndexedPlayerProvider players;
        protected override IPlayerProvider Players => players;

        public OnlineStartScreen(Assets assets, List<Team> teams, MultiplayerClient multiplayerClient,
                                 List<IController> controllers, Settings settings)
            : base(assets, teams, controllers, settings)
        {
            players = new IndexedPlayerProvider();

            this.multiplayerClient = multiplayerClient;
            multiplayerClient.OnPacketReceived += OnPacketReceived;
        }

        protected override bool AddPlayer(IController controller)
        {
            var player = new OnlineLocalPlayerState(controller);

            players.AddPlayer(player.Id, player);

            _ = player.RequestServer(multiplayerClient);

            return true;
        }

        private void OnPacketReceived(Packet packet)
        {
            if (packet.Message is LobbyInfo lobby)
            {
                multiplayerStartIn = lobby.StartIn;
                players.SetIncomingMessage(lobby.Players);
            }
            else if (packet.Message is GameInfo gi)
            {
                if (multiplayerStartIn < 30 && multiplayerStartIn != -1)
                {
                    multiplayerClient.OnPacketReceived -= OnPacketReceived;
                    ScreenManager.SetScreen(new OnlineGameScreen(assets, multiplayerClient, Players));
                }
            }
        }

        public override void Update()
        {
            base.Update();

            if (multiplayerStartIn > 0)
            {
                multiplayerStartIn--;
            }

            multiplayerClient.CheckPackets();

            _ = multiplayerClient.SendPacket(new Packet()
            {
                Lobby = multiplayerClient.LobbyId,
                ClientSecret = multiplayerClient.ClientSecret,
                Message = new PingMessage()
                {
                    PingId = 0,
                }
            });

            //foreach (OnlineLocalPlayerState player in playerIndex.EnumeratePendingPlayers().Cast<OnlineLocalPlayerState>())
            //{
            //    _ = player.RequestServer(multiplayerClient);
            //}

            if (multiplayerClient.IsDead())
            {
                ScreenManager.SetScreen(new OnlineConnectionDiedScreen(assets));
            }
        }

        public override void Draw(SpriteBatch ctx)
        {
            base.Draw(ctx);

            if (multiplayerStartIn >= 0)
            {
                Game.DrawString(ctx, 8, 200 - 20, multiplayerStartIn.ToString(), assets.Alpha[1]);
            }
        }

        protected override void Start()
        {
        }
    }
}
