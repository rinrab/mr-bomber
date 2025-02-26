// Copyright (c) Timofei Zhakov. All rights reserved.

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MrBoom.Common;

namespace MrBoom
{
    public abstract class AbstractGameScreen : IScreen
    {
        protected Terrain terrain;
        protected IClientTerrain clientTerrain;
        protected readonly List<Team> teams;
        protected readonly Assets assets;
        protected readonly Settings settings;
        protected readonly List<IController> controllers;
        protected bool isPause = false;

        private int bgTick = 0;

        private bool isF4Toggle = false;
        private bool f4Mask;

        public AbstractGameScreen(List<Team> teams, Assets assets, Settings settings, List<IController> controllers)
        {
            this.teams = teams;
            this.assets = assets;
            this.settings = settings;
            this.controllers = controllers;

            int levelIndex = ScreenManager.GetNextLevel();

            terrain = new Terrain(levelIndex, assets);

            ScreenManager.NextSong(assets.Sounds, MapData.Data[levelIndex].Song);
        }

        public virtual void Update()
        {
            bgTick++;

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
                terrain.Update();
                clientTerrain.ClientUpdate();

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

                PlaySounds(terrain.SoundsToPlay);
            }
        }

        public virtual void Draw(SpriteBatch ctx)
        {
            if (clientTerrain.LevelAssets.MovingBackground != null)
            {
                Image img = clientTerrain.LevelAssets.MovingBackground;
                int xCount = 320 / img.Width + 2;

                for (int y = 0; y < 5; y++)
                {
                    for (int x = 0; x < 8; x++)
                    {
                        img.Draw(ctx, img.Width * xCount - (clientTerrain.Tick / 2 + x * img.Width +
                            y * img.Height / 2) % (img.Width * xCount) - img.Width, y * img.Height);
                    }
                }
            }

            clientTerrain.LevelAssets.Backgrounds[bgTick / 20].Draw(ctx, 0, 0);
            foreach (var overlay in clientTerrain.LevelAssets.BackgroundSprites)
            {
                overlay.Images[bgTick / overlay.AnimationDelay].Draw(ctx, overlay.x, overlay.y);
            }

            for (int y = 0; y < clientTerrain.Height; y++)
            {
                for (int x = 0; x < clientTerrain.Width; x++)
                {
                    Cell cell = clientTerrain.GetCell(x, y);
                    AnimatedImage images = cell.GetImages(assets, clientTerrain.LevelAssets);

                    if (images != null)
                    {
                        int index = (cell.Index == -1) ? 0 : cell.Index;
                        Image image = images[index];

                        image.Draw(ctx, x * 16 + 8 + 8 - image.Width / 2 + cell.OffsetX, y * 16 + 16 - image.Height + cell.OffsetY);
                    }
                }
            }

            var spritesToDraw = new List<IClientSprite>(clientTerrain.Sprites);

            spritesToDraw.Sort((a, b) => a.Y - b.Y);

            foreach (IClientSprite sprite in spritesToDraw)
            {
                sprite.Draw(ctx);
            }

            foreach (var overlay in clientTerrain.LevelAssets.Overlays)
            {
                overlay.Images[bgTick / overlay.AnimationDelay].Draw(ctx, overlay.x, overlay.y);
            }

            int drawInStart = 60 * 30 - clientTerrain.ApocalypseSpeed * (clientTerrain.MaxApocalypse + 5);
            if (clientTerrain.TimeLeft > 30 * 60)
            {
                int time = (clientTerrain.TimeLeft - 30 * 60) / 60;

                int min = time / 60;
                int sec = time % 60;

                string str = min.ToString() + ":" + ((sec < 10) ? 0 + sec.ToString() : sec.ToString());
                int x = 270;
                foreach (char c in str)
                {
                    string alpha = "0123456789:";
                    int index = alpha.IndexOf(c);
                    assets.BigDigits[index].Draw(ctx, x, 182);
                    if (index == 10)
                    {
                        x += 9;
                    }
                    else
                    {
                        x += 14;
                    }
                }
            }
            else if (clientTerrain.TimeLeft < drawInStart)
            {
                int x = 30;
                int y = 20;
                if (clientTerrain.TimeLeft > drawInStart - 20 - assets.DrawGameIn.Height)
                {
                    y = drawInStart - clientTerrain.TimeLeft - assets.DrawGameIn.Height;
                }

                assets.DrawGameIn.Draw(ctx, x, y);

                int timeLeft = (clientTerrain.TimeLeft + clientTerrain.ApocalypseSpeed * clientTerrain.MaxApocalypse) / 60;
                int firstNumber = timeLeft / 10;
                int secondNumber = timeLeft % 10;

                assets.DrawGameInNumbers[firstNumber].Draw(ctx, x + 42, y + 15);
                assets.DrawGameInNumbers[secondNumber].Draw(ctx, x + 8 + 42, y + 15);
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

        public virtual void DrawHighDPI(SpriteBatch ctx, Rectangle rect, float scale, int graphicScale)
        {
            if (settings.IsDebug && isF4Toggle)
            {
                for (int y = 1; y < terrain.Height - 1; y++)
                {
                    for (int x = 1; x < terrain.Width - 1; x++)
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
