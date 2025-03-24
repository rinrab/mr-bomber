// Copyright (c) Timofei Zhakov. All rights reserved.

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MrBoom.Core;
using MrBoom.Core.Sprites;
using MrBoom.Core.Sprites.Interface;
using MrBoom.Core.Terrain;

namespace MrBoom
{
    public class TerrainGraphics : IServerGameEntity, IClientDrawableGameEntity
    {
        protected readonly ITerrainProxy terrain;
        private readonly ClientTerrainSpriteHost sprites;
        private readonly Assets.Level levelAssets;
        protected readonly Assets assets;

        private int bgTick = 0;

        public TerrainGraphics(ITerrainProxy terrain, ClientTerrainSpriteHost sprites, Assets assets, Assets.Level levelAssets)
        {
            this.terrain = terrain;
            this.sprites = sprites;
            this.levelAssets = levelAssets;
            this.assets = assets;
        }

        public void ServerUpdate()
        {
            bgTick++;
        }

        public virtual void Draw(SpriteBatch ctx)
        {
            if (levelAssets.MovingBackground != null)
            {
                Image img = levelAssets.MovingBackground;
                int xCount = 320 / img.Width + 2;

                for (int y = 0; y < 5; y++)
                {
                    for (int x = 0; x < 8; x++)
                    {
                        img.Draw(ctx, img.Width * xCount - (bgTick / 2 + x * img.Width +
                            y * img.Height / 2) % (img.Width * xCount) - img.Width, y * img.Height);
                    }
                }
            }

            levelAssets.Backgrounds[bgTick / 20].Draw(ctx, 0, 0);
            foreach (var overlay in levelAssets.BackgroundSprites)
            {
                overlay.Images[bgTick / overlay.AnimationDelay].Draw(ctx, overlay.x, overlay.y);
            }

            for (int y = 0; y < terrain.Height; y++)
            {
                for (int x = 0; x < terrain.Width; x++)
                {
                    Cell cell = terrain.GetCell(x, y);
                    AnimatedImage images = cell.GetImages(assets, levelAssets);

                    if (images != null)
                    {
                        int index = (cell.Index == -1) ? 0 : cell.Index;
                        Image image = images[index];

                        image.Draw(ctx, x * 16 + 8 + 8 - image.Width / 2 + cell.OffsetX, y * 16 + 16 - image.Height + cell.OffsetY);
                    }
                }
            }

            var spritesToDraw = new List<GameEntityBase>(sprites.Sprites);

            spritesToDraw.Sort((a, b) => a.GetService<ISpritePositionProvider>().Y - b.GetService<ISpritePositionProvider>().Y);

            foreach (GameEntityBase sprite in spritesToDraw)
            {
                sprite.GetService<IClientDrawableGameEntity>().Draw(ctx);
            }

            foreach (var overlay in levelAssets.Overlays)
            {
                overlay.Images[bgTick / overlay.AnimationDelay].Draw(ctx, overlay.x, overlay.y);
            }

            int drawInStart = 60 * 30 - terrain.ApocalypseSpeed * (terrain.MaxApocalypse + 5);
            if (terrain.TimeLeft > 30 * 60)
            {
                int time = (terrain.TimeLeft - 30 * 60) / 60;

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
            else if (terrain.TimeLeft < drawInStart)
            {
                int x = 30;
                int y = 20;
                if (terrain.TimeLeft > drawInStart - 20 - assets.DrawGameIn.Height)
                {
                    y = drawInStart - terrain.TimeLeft - assets.DrawGameIn.Height;
                }

                assets.DrawGameIn.Draw(ctx, x, y);

                int timeLeft = (terrain.TimeLeft + terrain.ApocalypseSpeed * terrain.MaxApocalypse) / 60;
                int firstNumber = timeLeft / 10;
                int secondNumber = timeLeft % 10;

                assets.DrawGameInNumbers[firstNumber].Draw(ctx, x + 42, y + 15);
                assets.DrawGameInNumbers[secondNumber].Draw(ctx, x + 8 + 42, y + 15);
            }
        }

        public void DrawHighDPI(SpriteBatch ctx, Rectangle rect, float scale, int graphicScale)
        {
        }
    }
}
