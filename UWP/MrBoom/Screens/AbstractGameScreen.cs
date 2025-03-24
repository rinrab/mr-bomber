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

        public AbstractGameScreen(List<Team> teams, Assets assets, Settings settings, List<IController> controllers)
        {
            this.teams = teams;
            this.assets = assets;
            this.settings = settings;
            this.controllers = controllers;

            int levelIndex = ScreenManager.GetNextLevel();

            terrain = new Terrain(levelIndex, ExtensibilityProvider.Default.Random);
            clientTerrain = new ClientTerrain(terrain.GetService<TerrainProxyProvider>(), assets);

            clientTerrain.AddSingleton(services => new DebugInfoController(
                settings,
                services.GetService<Assets>(),
                terrain.GetService<TerrainMap>(),
                terrain.GetService<CheatHost>(),
                terrain, clientTerrain));

            ScreenManager.NextSong(assets.Sounds, MapData.Data[levelIndex].Song);
        }

        public virtual void Update()
        {
            if (!isPause)
            {
                terrain.ServerUpdate();
                clientTerrain.ServerUpdate();

                // PlaySounds(terrain.SoundsToPlay);
            }
        }

        public virtual void Draw(SpriteBatch ctx)
        {
            clientTerrain.Draw(ctx);
        }

        public virtual void DrawHighDPI(SpriteBatch ctx, Rectangle rect, float scale, int graphicScale)
        {
            clientTerrain.DrawHighDPI(ctx, rect, scale, graphicScale);
        }
    }
}
