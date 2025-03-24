// Copyright (c) Timofei Zhakov. All rights reserved.

using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MrBoom.Core;
using MrBoom.Core.Service;
using MrBoom.Core.Terrain;
using MrBoom.Core.Terrain.Cheats;

namespace MrBoom
{
    public enum DebugInfoPage
    {
        None,
        General,
        Terrain,
        ServerSprites,
        ClientSprites,
        TravelCostGrid,
    }

    public class DebugInfoController : IClientDrawableGameEntity, IServerGameEntity
    {
        private readonly Settings settings;
        private readonly Assets assets;
        private readonly TerrainMap map;
        private readonly CheatHost cheats;
        private readonly Terrain terrain;
        private readonly ClientTerrain clientTerrain;

        private bool f4Mask;

        private DebugInfoPage page = DebugInfoPage.None;

        public DebugInfoController(Settings settings,
                                   Assets assets,
                                   TerrainMap map,
                                   CheatHost cheats,
                                   Terrain terrain,
                                   ClientTerrain clientTerrain)
        {
            this.settings = settings;
            this.assets = assets;
            this.map = map;
            this.cheats = cheats;
            this.terrain = terrain;
            this.clientTerrain = clientTerrain;
        }

        public void Draw(SpriteBatch ctx)
        {
        }

        public void DrawHighDPI(SpriteBatch ctx, Rectangle rect, float scale, int graphicScale)
        {
            if (page == DebugInfoPage.TravelCostGrid)
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
            }

            if (page == DebugInfoPage.General)
            {
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

            if (page == DebugInfoPage.Terrain)
            {
                DrawManyServicesDebugInfo(ctx, scale, new BomberServiceProvider[] { terrain, clientTerrain });
            }

            if (page == DebugInfoPage.ServerSprites)
            {
                DrawManyServicesDebugInfo(ctx, scale, terrain.GetService<TerrainSpriteHost>().GetSprites().Cast<BomberServiceProvider>());
            }

            if (page == DebugInfoPage.ClientSprites)
            {
                DrawManyServicesDebugInfo(ctx, scale, clientTerrain.GetService<ClientTerrainSpriteHost>().Sprites);
            }
        }

        private void DrawManyServicesDebugInfo(SpriteBatch ctx, float scale, IEnumerable<BomberServiceProvider> services)
        {
            Vector2 offset = new Vector2();

            foreach (var service in services)
            {
                offset.X += DrawServicesDebugInfo(ctx, scale, service, offset).X;
            }
        }

        private Vector2 DrawServicesDebugInfo(SpriteBatch ctx, float scale, BomberServiceProvider services, Vector2 offset)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine($"{services.GetType()}:");
            sb.AppendLine();

            foreach (object service in services.EnumerateServices<object>())
            {
                if (service is IServerGameEntity)
                {
                    sb.Append('U');
                }
                else
                {
                    sb.Append(' ');
                }

                if (service is IClientDrawableGameEntity)
                {
                    sb.Append('D');
                }
                else
                {
                    sb.Append(' ');
                }

                sb.Append(' ');
                sb.AppendLine(service.ToString());
            }

            return DrawText(ctx, scale, sb.ToString(), offset);
        }

        private Vector2 DrawText(SpriteBatch ctx, float scale, string text, Vector2 offset)
        {
            float textScale = scale / 6;
            Vector2 actualOffset = offset / 6 * scale;

            Vector2 size = (assets.DebugFont.MeasureString(text) + new Vector2(16)) * textScale;
            Rectangle area = new Rectangle((int)actualOffset.X, (int)actualOffset.Y, (int)size.X, (int)size.Y);
            ctx.Draw(assets.BlackPixel, area, Color.White * 0.7f);

            ctx.DrawString(assets.DebugFont,
                           text,
                           actualOffset,
                           Color.White,
                           0,
                           Vector2.Zero,
                           textScale,
                           SpriteEffects.None,
                           0);

            return size / textScale;
        }

        public void ServerUpdate()
        {
            var state = Keyboard.GetState();

            if (state.IsKeyDown(Keys.F4))
            {
                if (!f4Mask)
                {
                    page++;

                    if (page == DebugInfoPage.TravelCostGrid)
                    {
                        page = DebugInfoPage.None;
                    }
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
