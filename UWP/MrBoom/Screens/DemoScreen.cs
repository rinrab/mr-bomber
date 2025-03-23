// Copyright (c) Timofei Zhakov. All rights reserved.

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MrBoom.Bot;
using MrBoom.Core.Sprites;
using MrBoom.Core.Terrain;
using MrBoom.Screens;
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
                new TextMenuItem("PLAY"),
                new TextMenuItem("ONLINE"),
                new SelectMenuItem("TEAM", new string[]
                {
                    "OFF",
                    "COLOR",
                    "SEX",
                }) { SelectionIndex = (int)settings.TeamMode },
                new TextMenuItem("QUIT"),
            }, assets, controllers);

            TerrainSpriteHost sprites = terrain.GetService<TerrainSpriteHost>();
            ClientTerrainSpriteHost clientSprites = clientTerrain.GetService<ClientTerrainSpriteHost>();

            for (int i = 0; i < 4; i++)
            {
                sprites.AddPlayer(new ComputerPlayer(i, i, i));
            }

            sprites.InitializeMonsters();

            foreach (GameEntityBase sprite in sprites.GetSprites())
            {
                clientSprites.Sprites.Add(new ClientSprite(sprite.GetService<ISpriteProxy>(), assets));
            }
        }

        public override void Update()
        {
            base.Update();

            GameEndedHandler gameEndedHandler = terrain.GetService<GameEndedHandler>();

            if (gameEndedHandler.Result == GameResult.Victory || gameEndedHandler.Result == GameResult.Draw)
            {
                int levelIndex = ScreenManager.GetNextLevel();

                terrain = new Terrain(levelIndex, ExtensibilityProvider.Default.Random);

                TerrainSpriteHost sprites = terrain.GetService<TerrainSpriteHost>();

                ScreenManager.NextSong(assets.Sounds, MapData.Data[levelIndex].Song);

                for (int i = 0; i < 4; i++)
                {
                    sprites.AddPlayer(new ComputerPlayer(i, i, i));
                }

                sprites.InitializeMonsters();
            }

            demoMenu.Update();

            if (demoMenu.Action == 0)
            {
                settings.IsOnline = false;
                ScreenManager.SetScreen(new SinglePlayerStartScreen(assets, teams, controllers, settings));
            }
            else if (demoMenu.Action == 1)
            {
                settings.IsOnline = true;
                ScreenManager.SetScreen(new OnlineConnectScreen(assets, teams, controllers, settings));
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

        public override void Draw(SpriteBatch ctx)
        {
            base.Draw(ctx);

            demoMenu.Draw(ctx);
        }

        public override void DrawHighDPI(SpriteBatch ctx, Rectangle rect, float scale, int graphicScale)
        {
            base.DrawHighDPI(ctx, rect, scale, graphicScale);

            demoMenu.DrawHighDPI(ctx, rect, scale, graphicScale);
        }
    }
}
