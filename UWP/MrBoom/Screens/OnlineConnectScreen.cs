// Copyright (c) Timofei Zhakov. All rights reserved.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MrBoom.NetworkProtocol;
using MrBoom.NetworkProtocol.Messages;

namespace MrBoom.Screens
{
    public class OnlineConnectScreen : IScreen
    {
        private readonly Assets assets;
        private readonly List<Team> teams;
        private readonly List<IController> controllers;
        private readonly Settings settings;

        private readonly MultiplayerClient multiplayerClient;

        private string header;
        private string state;

        public OnlineConnectScreen(Assets assets, List<Team> teams, List<IController> controllers, Settings settings)
        {
            this.assets = assets;
            this.teams = teams;
            this.controllers = controllers;
            this.settings = settings;

            multiplayerClient = new MultiplayerClient();
            multiplayerClient.OnPacketReceived += OnPacketReceived;

            header = "connecting to the server";
            state = "matchmaking...";

            Task.Run(InitializeAsync);
        }

        private void OnPacketReceived(Packet packet)
        {
            state = "connected!";
            multiplayerClient.OnPacketReceived -= OnPacketReceived;

            ScreenManager.SetScreen(new OnlineStartScreen(assets, teams, multiplayerClient, controllers, settings));
        }

        private async Task InitializeAsync()
        {
            ClientJoinResponse lobby = await multiplayerClient.JoinLobby(new ClientJoinRequest());

            state = "joining lobby...";

            await multiplayerClient.ConnectLobby(lobby);

            state = "verifying connection...";

            await multiplayerClient.ListenAsync(default);
        }

        public void Update()
        {
            multiplayerClient.CheckPackets();
        }

        public void Draw(SpriteBatch ctx)
        {
            assets.MrFond.Draw(ctx, 0, 0);

            Game.DrawString(ctx, 320 / 2 - header.Length * 4, 0, header, assets.Alpha[1]);
            Game.DrawString(ctx, 320 / 2 - state.Length * 4, 200 - 8, state, assets.Alpha[1]);
        }

        public void DrawHighDPI(SpriteBatch ctx, Rectangle rect, float scale, int graphicScale)
        {
        }
    }
}
