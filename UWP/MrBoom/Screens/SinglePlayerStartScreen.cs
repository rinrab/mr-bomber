// Copyright (c) Timofei Zhakov. All rights reserved.

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MrBoom.Common;
using MrBoom.State;
using Windows.UI.Xaml;

namespace MrBoom.Screens
{
    public class SinglePlayerStartScreen : AbstractStartScreen
    {
        private readonly NameGenerator nameGenerator;
        private TeamMode teamMode = 0;
        private Menu menu;

        public SinglePlayerStartScreen(Assets assets, List<Team> teams,
                                       List<IController> controllers, Settings settings)
            : base(assets, teams, controllers, settings)
        {
            nameGenerator = new NameGenerator(Terrain.Random);
            teamMode = settings.TeamMode;
        }

        protected override IPlayerState CreatePlayer(int index, IController controller)
        {
            return new SinglePlayerHumanPlayerState(controller, index, nameGenerator.GenerateName());
        }

        protected override void Start()
        {
            if (players.Count == 1)
            {
                players.AddPlayer(index => new SinglePlayerBotPlayerState(index, "bot"));
            }
            else if (players.Count == 0)
            {
                for (int i = 0; i < 8; i++)
                {
                    players.AddPlayer(index => new SinglePlayerBotPlayerState(index, "bot"));
                }
            }

            if (players.Count >= 1)
            {
                if (players.Count == 1 && teamMode != 0)
                {
                    teamMode = 0;
                }

                settings.TeamMode = teamMode;

                teams.Clear();
                if (teamMode == TeamMode.Off)
                {
                    foreach (IPlayerState player in players.EnumeratePlayers())
                    {
                        teams.Add(new Team { Players = new List<IPlayerState> { player } });
                    }
                }
                if (teamMode == TeamMode.Color)
                {
                    if (players.Count == 2)
                    {
                        foreach (IPlayerState player in players.EnumeratePlayers())
                        {
                            teams.Add(new Team { Players = new List<IPlayerState> { player } });
                        }
                    }
                    else
                    {
                        for (int i = 0; i < players.Count; i += 2)
                        {
                            var newPlayers = new List<IPlayerState> { players[i] };
                            if (i + 1 < players.Count)
                            {
                                newPlayers.Add(players[i + 1]);
                            }

                            teams.Add(new Team { Players = newPlayers });
                        }
                    }
                }
                if (teamMode == TeamMode.Sex)
                {
                    teams.Add(new Team { Players = new List<IPlayerState>() });
                    teams.Add(new Team { Players = new List<IPlayerState>() });

                    for (int i = 0; i < players.Count; i += 2)
                    {
                        teams[0].Players.Add(players[i]);
                        if (i + 1 < players.Count)
                        {
                            teams[1].Players.Add(players[i + 1]);
                        }
                    }
                }

                ScreenManager.SetScreen(new GameScreen(teams, assets, settings, controllers));
            }
        }

        public override void Update()
        {
            if (menu == null)
            {
                base.Update();

                if (Controller.IsKeyDown(controllers, PlayerKeys.Menu))
                {
                    var options = new IMenuItem[] {
                        new TextMenuItem("START"),
                        new SelectMenuItem("TEAM", new string[] { "OFF", "COLOR", "SEX" })
                        {
                            SelectionIndex = (int)teamMode
                        },
                        new TextMenuItem("QUIT")
                    };

                    menu = new Menu(options, assets, controllers);
                    Controller.Reset(controllers);
                }

                if (Controller.IsKeyDown(controllers, PlayerKeys.AddBot))
                {
                    if (players.AddPlayer(index => new SinglePlayerBotPlayerState(index, "bot")))
                    {
                        assets.Sounds.Addbot.Play();
                        Controller.Reset(controllers);
                    }
                }
            }
            else
            {
                menu.Update();

                teamMode = (TeamMode)((SelectMenuItem)menu.Items[1]).SelectionIndex;

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
                    Application.Current.Exit();
                }
            }
        }

        public override void Draw(SpriteBatch ctx)
        {
            base.Draw(ctx);

            int ox = 10;
            int oy = 10;

            assets.Controls[0].Draw(ctx, ox, oy);
            Game.DrawString(ctx, ox + 14 + 8, oy + 5, "or", assets.Alpha[1]);
            assets.Controls[1].Draw(ctx, ox + 14 + 8 * 4, oy + 1);
            Game.DrawString(ctx, ox + 14 + 25 + 8 * 5, oy + 5, "- join", assets.Alpha[1]);

            const int offset = 20;

            assets.Controls[2].Draw(ctx, ox, oy + offset);
            Game.DrawString(ctx, ox + 14 + 8, oy + 5 + offset, "or", assets.Alpha[1]);
            assets.Controls[3].Draw(ctx, ox + 14 + 8 * 4, oy + 1 + offset);
            Game.DrawString(ctx, ox + 14 * 2 + 8 * 5, oy + 5 + offset, "- add bot", assets.Alpha[1]);

            Game.DrawString(ctx, 320 - ox - 15 * 8, oy + 5,
                            "team mode: " + teamMode.ToString().ToLower(), assets.Alpha[1]);

            if (startTick >= 0)
            {
                double scale = Math.Abs(Math.Sin((double)startTick / 15)) * 0.5 + 1;

                int width = (int)(assets.StartButton.Width * scale);
                int height = (int)(assets.StartButton.Height * scale);

                Rectangle rect = new Rectangle(325 - width / 2, 38 - height / 2, width, height);

                ctx.Draw(assets.StartButton, rect, Color.White);
            }
            if (startTick >= 600)
            {
                string text = "press a or enter";
                text = text.Substring(0, Math.Min((startTick - 600) / 6, text.Length));

                Game.DrawString(ctx, (320 - text.Length * 8) / 2, 200 - 10, text, assets.Alpha[1]);
            }

            menu?.Draw(ctx);
        }

        public override void DrawHighDPI(SpriteBatch ctx, Rectangle rect, float scale, int graphicScale)
        {
            base.DrawHighDPI(ctx, rect, scale, graphicScale);

            menu?.DrawHighDPI(ctx, rect, scale, graphicScale);
        }
    }
}
