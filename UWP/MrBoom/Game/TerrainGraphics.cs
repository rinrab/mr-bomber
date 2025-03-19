// Copyright (c) Timofei Zhakov. All rights reserved.

using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using MrBoom.Core.Sprites;

namespace MrBoom
{
    public class TerrainGraphics : IServerGameEntity, IClientDrawableGameEntity
    {
        protected readonly ClientTerrain clientTerrain;
        protected readonly Assets assets;

        private int bgTick = 0;

        public TerrainGraphics(ClientTerrain clientTerrain, Assets assets)
        {
            this.clientTerrain = clientTerrain;
            this.assets = assets;
        }

        public void ServerUpdate()
        {
            bgTick++;
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

            var spritesToDraw = new List<GameEntityBase>(clientTerrain.Sprites);

            spritesToDraw.Sort((a, b) => a.GetService<ISpritePositionProvider>().Y - b.GetService<ISpritePositionProvider>().Y);

            foreach (GameEntityBase sprite in spritesToDraw)
            {
                sprite.GetService<IClientDrawableGameEntity>().Draw(ctx);
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
    }
}
