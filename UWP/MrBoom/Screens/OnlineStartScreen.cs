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
    public class OnlineStartScreen : AbstractStartScreen
    {
        private readonly MultiplayerClient multiplayerClient;
        private int multiplayerStartIn = -1;

        private readonly IDictionary<Guid, IPlayerState> playerIndex;
        private readonly IDictionary<Guid, OnlineLocalPlayerState> pendingPlayers;

        public OnlineStartScreen(Assets assets, List<Team> teams, MultiplayerClient multiplayerClient,
                                 List<IController> controllers, Settings settings)
            : base(assets, teams, controllers, settings)
        {
            playerIndex = new Dictionary<Guid, IPlayerState>();
            pendingPlayers = new Dictionary<Guid, OnlineLocalPlayerState>();

            this.multiplayerClient = multiplayerClient;
            multiplayerClient.OnPacketReceived += OnPacketReceived;
        }

        protected override IPlayerState CreatePlayer(int index, IController controller)
        {
            var player = new OnlineLocalPlayerState(controller);

            playerIndex.Add(player.Id, player);
            pendingPlayers.Add(player.Id, player);

            return player;
        }

        private void OnPacketReceived(Packet packet)
        {
            if (packet.Message is LobbyInfo lobby)
            {
                multiplayerStartIn = lobby.StartIn;

                players.Clear();

                for (int i = 0; i < lobby.Players.Count; i++)
                {
                    var player = lobby.Players[i];

                    if (playerIndex.TryGetValue(player.Id, out IPlayerState val))
                    {
                        if (val is OnlineLocalPlayerState onlinePlayer)
                        {
                            onlinePlayer.OnLoaded(player);
                            pendingPlayers.Remove(player.Id);
                        }

                        players.AddPlayer(_ => val);
                    }
                    else
                    {
                        players.AddPlayer(_ => new OnlineRemotePlayerState(player));
                    }
                }
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

            foreach (var player in pendingPlayers)
            {
                _ = player.Value.RequestServer(multiplayerClient);
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
    }
}
