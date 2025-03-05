// Copyright (c) Timofei Zhakov. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MrBoom.NetworkProtocol.Messages;
using MrBoom.State;

namespace MrBoom.Screens
{
    public class IndexedPlayerProvider<KeyT>
    {
        private readonly IDictionary<KeyT, IPlayerState> playerIndex;
        private readonly IDictionary<KeyT, IPlayerState> pendingPlayers;
        private IList<KeyT> playersToShow;

        public IndexedPlayerProvider()
        {
            playerIndex = new Dictionary<KeyT, IPlayerState>();
            pendingPlayers = new Dictionary<KeyT, IPlayerState>();
            playersToShow = new List<KeyT>();
        }

        public void IndexedAdd(KeyT key, IPlayerState player)
        {
            playerIndex.Add(key, player);
            pendingPlayers.Add(key, player);
        }

        public IEnumerable<IPlayerState> EnumeratePlayers()
        {
            foreach (KeyT player in playersToShow)
            {
                yield return playerIndex[player];
                pendingPlayers.Remove(player);
            }

            foreach (IPlayerState player in pendingPlayers.Values)
            {
                yield return player;
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

        public IEnumerable<IPlayerState> EnumeratePendingPlayers()
        {
            return pendingPlayers.Values;
        }
    }

    public class OnlineStartScreen : AbstractStartScreen
    {
        private readonly MultiplayerClient multiplayerClient;
        private int multiplayerStartIn = -1;

        private readonly IndexedPlayerProvider<Guid> playerIndex;

        public OnlineStartScreen(Assets assets, List<Team> teams, MultiplayerClient multiplayerClient,
                                 List<IController> controllers, Settings settings)
            : base(assets, teams, controllers, settings)
        {
            playerIndex = new IndexedPlayerProvider<Guid>();

            this.multiplayerClient = multiplayerClient;
            multiplayerClient.OnPacketReceived += OnPacketReceived;
        }

        protected override IPlayerState CreatePlayer(int index, IController controller)
        {
            var player = new OnlineLocalPlayerState(controller);

            playerIndex.IndexedAdd(player.Id, player);

            return player;
        }

        private void OnPacketReceived(Packet packet)
        {
            if (packet.Message is LobbyInfo lobby)
            {
                multiplayerStartIn = lobby.StartIn;

                playerIndex.Vacuum(players);
            }
            else if (packet.Message is GameInfo gi)
            {
                if (multiplayerStartIn < 30 && multiplayerStartIn != -1)
                {
                    multiplayerClient.OnPacketReceived -= OnPacketReceived;
                    ScreenManager.SetScreen(new OnlineGameScreen(assets, multiplayerClient, players));
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

            foreach (OnlineLocalPlayerState player in playerIndex.EnumeratePendingPlayers().Cast<OnlineLocalPlayerState>())
            {
                _ = player.RequestServer(multiplayerClient);
            }

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
