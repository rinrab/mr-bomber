// Copyright (c) Timofei Zhakov. All rights reserved.

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MrBoom.Core;
using MrBoom.Core.Terrain;
using MrBoom.Core.Terrain.Cheats;
using MrBoom.Screens;

namespace MrBoom
{
    public abstract class AbstractGameScreen : IScreen
    {
        protected Terrain terrain;
        protected ClientTerrain clientTerrain;
        protected Assets assets;

        protected readonly List<Team> teams;

        protected readonly Settings settings;
        protected readonly List<IController> controllers;
        protected bool isPause = false;

        private bool isF4Toggle = false;
        private bool f4Mask;

        public AbstractGameScreen(List<Team> teams, Assets assets, Settings settings, List<IController> controllers)
        {
            this.teams = teams;
            this.assets = assets;
            this.settings = settings;
            this.controllers = controllers;

            int levelIndex = ScreenManager.GetNextLevel();

            terrain = new Terrain(levelIndex, ExtensibilityProvider.Default.Random);
            clientTerrain = new ClientTerrain(terrain.GetService<TerrainProxyProvider>(), assets);

            ScreenManager.NextSong(assets.Sounds, MapData.Data[levelIndex].Song);
        }

        public virtual void Update()
        {
            var state = Keyboard.GetState();

            if (state.IsKeyDown(Keys.F4))
            {
                if (!f4Mask)
                {
                    isF4Toggle = !isF4Toggle;
                }
                f4Mask = true;
            }
            else
            {
                f4Mask = false;
            }

            if (!isPause)
            {
                terrain.ServerUpdate();
                clientTerrain.ServerUpdate();

                terrain.GetService<BasicSoundController>().ResetSounds();

                if (settings.IsDebug)
                {
                    CheatHost cheats = terrain.GetService<CheatHost>();

                    foreach (Keys key in state.GetPressedKeys())
                    {
                        if (Keys.F1 <= key && key <= Keys.F12)
                        {
                            cheats.ApplyCheat((int)key - (int)Keys.F1);
                        }
                    }
                }

                // PlaySounds(terrain.SoundsToPlay);
            }
        }

        public virtual void Draw(SpriteBatch ctx)
        {
            clientTerrain.Draw(ctx);
        }

        public virtual void DrawHighDPI(SpriteBatch ctx, Rectangle rect, float scale, int graphicScale)
        {
            if (settings.IsDebug && isF4Toggle)
            {
                var map = terrain.GetService<TerrainMap>();

                for (int y = 1; y < map.Height - 1; y++)
                {
                    for (int x = 1; x < map.Width - 1; x++)
                    {
                        string debugInfo = terrain.GetCellDebugInfo(x, y);

                        Vector2 size = assets.DebugFont.MeasureString(debugInfo) / 6 / graphicScale;

                        Vector2 position =
                            (new Vector2(x, y) * 16 + new Vector2(8 + 8, 0 + 8) - size / 2) *
                            graphicScale * scale + new Vector2(rect.X, rect.Y);

                        ctx.DrawString(assets.DebugFont,
                                       debugInfo,
                                       position,
                                       Color.White,
                                       0,
                                       Vector2.One / 2,
                                       scale / 6,
                                       SpriteEffects.None,
                                       0);
                    }
                }

                string text = terrain.GetDebugInfo();

                Vector2 debugInfoSize = (assets.DebugFont.MeasureString(text) + new Vector2(16)) / 6 * scale;
                Rectangle area = new Rectangle(0, 0, (int)debugInfoSize.X, (int)debugInfoSize.Y);
                ctx.Draw(assets.BlackPixel, area, Color.White * 0.7f);

                ctx.DrawString(assets.DebugFont,
                               text,
                               Vector2.Zero,
                               Color.White,
                               0,
                               Vector2.Zero,
                               scale / 6,
                               SpriteEffects.None,
                               0);
            }
        }
    }
}
