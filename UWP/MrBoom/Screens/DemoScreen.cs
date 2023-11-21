﻿// Copyright (c) Timofei Zhakov. All rights reserved.

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MrBoom.Bot;
using Windows.UI.Xaml;

namespace MrBoom
{
    public class DemoScreen : AbstractGameScreen
    {
        private readonly Menu demoMenu;

        public DemoScreen(List<Team> teams, Assets assets, Settings settings,
                          List<IController> controllers) : base(teams, assets, settings, controllers)
        {
            demoMenu = new Menu(new IMenuItem[]
            {
                new TextMenuItem("PLAY LOCAL"),
                new TextMenuItem("PLAY ONLINE"),
                new SelectMenuItem("TEAM", new string[]
                {
                    "OFF",
                    "COLOR",
                    "SEX",
                }) { SelectionIndex = (int)settings.TeamMode },
                new TextMenuItem("QUIT"),
            }, assets, controllers);

            for (int i = 0; i < 4; i++)
            {
                terrain.AddPlayer(new ComputerPlayer(terrain, assets.Players[i], i, i));
            }

            terrain.InitializeMonsters();
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();
            if (terrain.Result == GameResult.Victory || terrain.Result == GameResult.Draw)
            {
                int levelIndex = ScreenManager.GetNextLevel();

                terrain = new Terrain(levelIndex, assets);

                ScreenManager.NextSong(assets.Sounds, MapData.Data[levelIndex].Song);

                for (int i = 0; i < 4; i++)
                {
                    terrain.AddPlayer(new ComputerPlayer(terrain, assets.Players[i], i, i));
                }

                terrain.InitializeMonsters();
            }

            demoMenu.Update();

            if (demoMenu.Action == 0)
            {
                ScreenManager.SetScreen(new MultiplayerStartScreen(assets, teams, controllers, settings));
            }
            else if (demoMenu.Action == 1)
            {
                ScreenManager.SetScreen(new OnlineStartScreen(assets, teams, controllers, new List<HumanPlayerState>(), settings));
            }
            else if (demoMenu.Action == 3)
            {
                Application.Current.Exit();
            }

            SelectMenuItem teamModeMenuItem = (SelectMenuItem)demoMenu.Items[2];
            settings.TeamMode = (TeamMode)teamModeMenuItem.SelectionIndex;

            if (Controller.IsKeyDown(controllers, PlayerKeys.Continue))
            {
                 Controller.Reset(controllers);
            }
        }

        protected override void OnDraw(SpriteBatch ctx)
        {
            base.OnDraw(ctx);

            demoMenu.Draw(ctx);
        }

        protected override void OnDrawHighDPI(SpriteBatch ctx, Rectangle rect, float scale, int graphicScale)
        {
            base.OnDrawHighDPI(ctx, rect, scale, graphicScale);

            demoMenu.DrawHighDPI(ctx, rect, scale, graphicScale);
        }
    }
}
