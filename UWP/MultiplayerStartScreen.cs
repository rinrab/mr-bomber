﻿// Copyright (c) Timofei Zhakov. All rights reserved.

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Windows.ApplicationModel;
using Windows.UI.Xaml;

namespace MrBoom
{
    public class MultiplayerStartScreen : IScreen
    {
        public Screen Next { get; private set; }

        private int tick = 0;

        private readonly Assets assets;
        private readonly List<Team> teams;
        private readonly List<IController> controllers;
        private readonly List<IController> unjoinedControllers;
        private readonly List<IController> joinedControllers;
        private readonly string helpText =
            "welcome to mr.bomber " +
            $"v{Package.Current.Id.Version.Major}.{Package.Current.Id.Version.Minor}" +
            $".{Package.Current.Id.Version.Build}!!!   " +
            "players can join using their drop bomb button second press or enter will start game   " +
            "gamepad controller: d-pad or left stick - move  a button - drop bomb  b button radio control   " +
            "right keyboard: arrows - move  ctrl - drop  bomb  shift - radio control   " +
            "left keyboard: wsad - move  ctrl - drop  bomb  shift - radio control   ";

        private int startTick = -1;
        private int teamMode = 0;
        private int playersCount;
        private readonly List<PlayerState> players;
        private Menu menu;

        public MultiplayerStartScreen(Assets assets, List<Team> teams, List<IController> controllers)
        {
            this.assets = assets;
            this.teams = teams;
            this.controllers = controllers;
            this.unjoinedControllers = new List<IController>(controllers);
            this.joinedControllers = new List<IController>();
            players = new List<PlayerState>();
            teamMode = Team.Mode;

            teams.Clear();
        }

        public void Draw(SpriteBatch ctx)
        {
            assets.Start.Draw(ctx, 0, 0);

            int ox = 10;
            int oy = 10;

            assets.Controls[0].Draw(ctx, ox, oy);
            Game.DrawString(ctx, ox + 20, oy + 5, "or", assets.Alpha[1]);
            assets.Controls[1].Draw(ctx, ox + 40, oy + 1);
            Game.DrawString(ctx, ox + 70, oy + 5, "join", assets.Alpha[1]);

            string[] teamModes = new string[] { "off", "color", "sex" };
            Game.DrawString(ctx, 320 - ox - 15 * 8, oy + 5,
                            "team mode: " + teamModes[teamMode], assets.Alpha[1]);

            for (int x = 0; x < 4; x++)
            {
                for (int y = 0; y < 2; y++)
                {
                    int index = y * 4 + x;
                    AnimatedImage images = assets.Alpha[index / 2 + 2];
                    if (index < players.Count)
                    {
                        PlayerState player = players[index];

                        Game.DrawString(ctx, 13 + x * 80, 78 + y * 70, "name ?", images);
                        Game.DrawString(ctx, 21 + x * 80, 88 + y * 70, player.Name, images);
                    }
                    else
                    {
                        if (tick / 30 % 4 == 0)
                        {
                            Game.DrawString(ctx, x * 80 + 20, y * 70 + 78, "join", images);
                            Game.DrawString(ctx, x * 80 + 28, y * 70 + 88, "us", images);
                            Game.DrawString(ctx, x * 80 + 28, y * 70 + 98, "!!", images);
                        }
                        else if (tick / 30 % 4 == 2)
                        {
                            Game.DrawString(ctx, x * 80 + 20, y * 70 + 78, "push", images);
                            Game.DrawString(ctx, x * 80 + 20, y * 70 + 88, "fire", images);
                            Game.DrawString(ctx, x * 80 + 28, y * 70 + 98, "!!", images);
                        }
                    }
                }
            }

            Game.DrawString(ctx, 320 - tick % (helpText.Length * 8 + 320), 192, helpText, assets.Alpha[1]);

            if (startTick >= 0)
            {
                double scale = Math.Abs(Math.Sin((double)startTick / 15)) * 0.5 + 1;

                int width = (int)(assets.StartButton.Width * scale);
                int height = (int)(assets.StartButton.Height * scale);

                Rectangle rect = new Rectangle(300 - width / 2, 38 - height / 2, width, height);

                ctx.Draw(assets.StartButton, rect, Color.White);
            }
            if (startTick >= 600)
            {
                string text = "press a or enter";
                text = text.Substring(0, Math.Min((startTick - 600) / 6, text.Length));

                Game.DrawString(ctx, (300 - text.Length * 8) / 2, 36, text, assets.Alpha[1]);
            }

            menu?.Draw(ctx);
        }

        public void Update()
        {
            tick++;

            if (menu == null)
            {
                List<IController> toRemove = new List<IController>();
                foreach (IController controller in unjoinedControllers)
                {
                    if (controller.IsKeyDown(PlayerKeys.Bomb))
                    {
                        string[] names = new string[]
                        {
                            "gin", "jai", "jay", "lad", "dre", "ash", "zev", "buz", "nox", "oak",
                            "coy", "eza", "fil", "kip", "aya", "jem", "roy", "rex", "ryu", "gus",
                            "cpp", "sus", "god", "guy", "bob", "jim", "mrb", "max"
                        };
                        string name = names[Terrain.Random.Next(names.Length)];

                        players.Add(new PlayerState(controller, playersCount) { Name = name });
                        playersCount++;
                        assets.Sounds.Addplayer.Play();

                        toRemove.Add(controller);
                    }
                }

                foreach (IController controller in toRemove)
                {
                    controller.Reset();
                    controller.Update();
                    unjoinedControllers.Remove(controller);
                    joinedControllers.Add(controller);
                }

                if (Controller.IsKeyDown(controllers, PlayerKeys.Menu))
                {
                    var options = new IMenuItem[] {
                        new TextMenuItem("START"),
                        new SelectMenuItem("TEAM", new string[] { "OFF", "COLOR", "SEX" })
                        {
                            SelectionIndex = teamMode
                        },
                        new TextMenuItem("HOME"),
                        new TextMenuItem("QUIT")
                    };

                    menu = new Menu(options, assets, controllers, 160);
                    Controller.Reset(controllers);
                }

                if (Controller.IsKeyDown(controllers, PlayerKeys.StartGame) ||
                    Controller.IsKeyDown(joinedControllers, PlayerKeys.Bomb))
                {
                    Start();
                }

                if (startTick == -1)
                {
                    if (players.Count >= 1)
                    {
                        startTick = 0;
                    }
                }
                else
                {
                    startTick++;
                }
            }
            else
            {
                menu.Update();

                teamMode = ((SelectMenuItem)menu.Items[1]).SelectionIndex;

                if (menu.Action == -2)
                {
                    menu = null;
                    Controller.Reset(controllers);
                }
                else if (menu.Action == 0)
                {
                    Start();
                }
                else if (menu.Action == 2)
                {
                    ScreenManager.SetScreen(new StartScreen(assets, teams, controllers));
                }
                else if (menu.Action == 3)
                {
                    Application.Current.Exit();
                }
            }
        }

        private void Start()
        {
            if (players.Count >= 1)
            {
                if (players.Count == 1 && teamMode != 0)
                {
                    teamMode = 0;
                }

                Team.Mode = teamMode;

                teams.Clear();
                if (teamMode == 0)
                {
                    foreach (PlayerState player in players)
                    {
                        teams.Add(new Team { Players = new List<PlayerState> { player } });
                    }
                }
                if (teamMode == 1)
                {
                    if (players.Count == 2)
                    {
                        teams.Add(new Team
                        {
                            Players = new List<PlayerState>()
                            {
                                new PlayerState(players[0].Controller, 0) { Name = players[0].Name }
                            }
                        });
                        teams.Add(new Team
                        {
                            Players = new List<PlayerState>()
                            {
                                new PlayerState(players[1].Controller, 2) { Name = players[1].Name }
                            }
                        });
                    }
                    else
                    {
                        for (int i = 0; i < players.Count; i += 2)
                        {
                            var newPlayers = new List<PlayerState> { players[i] };
                            if (i + 1 < players.Count)
                            {
                                newPlayers.Add(players[i + 1]);
                            }

                            teams.Add(new Team { Players = newPlayers });
                        }
                    }
                }
                if (teamMode == 2)
                {
                    teams.Add(new Team { Players = new List<PlayerState>() });
                    teams.Add(new Team { Players = new List<PlayerState>() });

                    for (int i = 0; i < players.Count; i += 2)
                    {
                        teams[0].Players.Add(players[i]);
                        if (i + 1 < players.Count)
                        {
                            teams[1].Players.Add(players[i + 1]);
                        }
                    }
                }

                Next = Screen.Game;
            }
        }  

        public void DrawHighDPI(SpriteBatch ctx, Rectangle rect, float scale)
        {
            menu?.DrawHighDPI(ctx, rect, scale);

            //ctx.DrawString(assets.MenuFontBig, "High DPI test",
            //               new Vector2(rect.X + 100 * scale, rect.Y + 100 * scale),
            //               Color.Red, 0, Vector2.Zero, scale / 6, SpriteEffects.None, 0);
        }
    }
}
