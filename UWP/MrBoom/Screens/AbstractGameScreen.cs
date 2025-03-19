// Copyright (c) Timofei Zhakov. All rights reserved.

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MrBoom.Core.Terrain;
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

                if (settings.IsDebug)
                {
                    if (state.IsKeyDown(Keys.F1))
                    {
                        terrain.DetonateAll(true);
                    }
                    if (state.IsKeyDown(Keys.F2))
                    {
                        terrain.DetonateAll(false);
                    }
                    if (state.IsKeyDown(Keys.F3))
                    {
                        terrain.StartApocalypse();
                    }
                    if (state.IsKeyDown(Keys.F5))
                    {
                        terrain.GiveAll();
                    }
                }

                // PlaySounds(terrain.SoundsToPlay);
            }
        }

        private void PlaySounds(SoundEffectType soundsToPlay)
        {
            var soundAssets = assets.Sounds;
            if (soundsToPlay.HasFlag(SoundEffectType.Bang)) soundAssets.Bang.Play();
            if (soundsToPlay.HasFlag(SoundEffectType.PoseBomb)) soundAssets.PoseBomb.Play();
            if (soundsToPlay.HasFlag(SoundEffectType.Sac)) soundAssets.Sac.Play();
            if (soundsToPlay.HasFlag(SoundEffectType.Pick)) soundAssets.Pick.Play();
            if (soundsToPlay.HasFlag(SoundEffectType.PlayerDie)) soundAssets.PlayerDie.Play();
            if (soundsToPlay.HasFlag(SoundEffectType.Oioi)) soundAssets.Oioi.Play();
            if (soundsToPlay.HasFlag(SoundEffectType.Ai)) soundAssets.Ai.Play();
            if (soundsToPlay.HasFlag(SoundEffectType.Addplayer)) soundAssets.Addplayer.Play();
            if (soundsToPlay.HasFlag(SoundEffectType.Victory)) soundAssets.Victory.Play();
            if (soundsToPlay.HasFlag(SoundEffectType.Draw)) soundAssets.Draw.Play();
            if (soundsToPlay.HasFlag(SoundEffectType.Clock)) soundAssets.Clock.Play();
            if (soundsToPlay.HasFlag(SoundEffectType.TimeEnd)) soundAssets.TimeEnd.Play();
            if (soundsToPlay.HasFlag(SoundEffectType.Skull)) soundAssets.Skull.Play();
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
