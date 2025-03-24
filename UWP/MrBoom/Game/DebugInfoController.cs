// Copyright (c) Timofei Zhakov. All rights reserved.

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MrBoom.Core;
using MrBoom.Core.Terrain;
using MrBoom.Core.Terrain.Cheats;

namespace MrBoom
{
    public class DebugInfoController : IClientDrawableGameEntity, IServerGameEntity
    {
        private readonly Settings settings;
        private readonly Assets assets;
        private readonly TerrainMap map;
        private readonly CheatHost cheats;
        private readonly Terrain terrain;

        private bool isF4Toggle = false;
        private bool f4Mask;

        public DebugInfoController(Settings settings,
                                   Assets assets,
                                   TerrainMap map,
                                   CheatHost cheats,
                                   Terrain terrain)
        {
            this.settings = settings;
            this.assets = assets;
            this.map = map;
            this.cheats = cheats;
            this.terrain = terrain;
        }

        public void Draw(SpriteBatch ctx)
        {
        }

        public void DrawHighDPI(SpriteBatch ctx, Rectangle rect, float scale, int graphicScale)
        {
            if (settings.IsDebug && isF4Toggle)
            {
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

        public void ServerUpdate()
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

            if (settings.IsDebug)
            {
                foreach (Keys key in state.GetPressedKeys())
                {
                    if (Keys.F1 <= key && key <= Keys.F12)
                    {
                        cheats.ApplyCheat((int)key - (int)Keys.F1);
                    }
                }
            }
        }
    }
}
