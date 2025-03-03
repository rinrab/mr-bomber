// Copyright (c) Timofei Zhakov. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MrBoom.NetworkProtocol.Messages;

namespace MrBoom.Screens
{
    public class OnlineStartScreen : AbstractStartScreen
    {
        private readonly MultiplayerClient multiplayerClient;
        private int multiplayerStartIn = -1;
        private readonly IDictionary<Guid, IPlayerState> playerIndex;

        public OnlineStartScreen(Assets assets, List<Team> teams, MultiplayerClient multiplayerClient,
                                 List<IController> controllers, Settings settings)
            : base(assets, teams, controllers, settings)
        {
            playerIndex = new Dictionary<Guid, IPlayerState>();

            this.multiplayerClient = multiplayerClient;
            multiplayerClient.OnPacketReceived += OnPacketReceived;
        }

        protected override IPlayerState CreatePlayer(int index, IController controller)
        {
            var player = new OnlinePlayerState(controller);
            playerIndex.Add(player.Id, player);
            _ = player.RequestServer(multiplayerClient);
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
                        if (val is OnlinePlayerState onlinePlayer)
                        {
                            onlinePlayer.OnLoaded(player);
                        }

                        players.Add(val);
                    }
                    else
                    {
                        players.Add(new OnlineRemotePlayerState(player));
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
