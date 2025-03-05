// Copyright (c) Timofei Zhakov. All rights reserved.

using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MrBoom.State;

namespace MrBoom
{
    public abstract class AbstractStartScreen : IScreen
    {
        private int tick = 0;

        protected readonly Assets assets;
        protected List<Team> teams;
        protected readonly List<IController> controllers;
        protected readonly Settings settings;
        private readonly List<IController> unjoinedControllers;
        private readonly List<IController> joinedControllers;

        private readonly string helpText =
            "welcome to mr.bomber " +
            $"v{Version.VersionString}!!!   " +
            "players can join using their drop bomb button second press or enter will start game   " +
            "gamepad controller: d-pad or left stick - move  a button - drop bomb  b button radio control   " +
            "right keyboard: arrows - move  ctrl - drop  bomb  shift - radio control   " +
            "left keyboard: wsad - move  ctrl - drop  bomb  shift - radio control   ";

        protected int startTick = -1;

        protected abstract IPlayerProvider Players { get; }

        public AbstractStartScreen(Assets assets, List<Team> teams, List<IController> controllers, Settings settings)
        {
            this.assets = assets;
            this.teams = teams;
            this.controllers = controllers;
            this.settings = settings;

            unjoinedControllers = new List<IController>(controllers);
            joinedControllers = new List<IController>();

            teams.Clear();
        }

        public virtual void Draw(SpriteBatch ctx)
        {
            assets.Start.Draw(ctx, 0, 0);

            var players = Players.ToList();

            for (int x = 0; x < 4; x++)
            {
                for (int y = 0; y < 2; y++)
                {
                    int index = y * 4 + x;
                    AnimatedImage images = assets.Alpha[index / 2 + 2];
                    if (index < players.Count)
                    {
                        Game.DrawString(ctx, 13 + x * 80, 78 + y * 70, "name ?", images);
                        Game.DrawString(ctx, 21 + x * 80, 88 + y * 70, players[index].Name, images);
                    }
                    else
                    {
                        if (tick / 30 % 4 == 0)
                        {
                            Game.DrawString(ctx, x * 80 + 20, y * 70 + 78, "join", images);
                            Game.DrawString(ctx, x * 80 + 28, y * 70 + 88, "us", images);
                            Game.DrawString(ctx, x * 80 + 28, y * 70 + 98, "!!", images);
                        }
                        else if (tick / 30 % 4 == 2)
                        {
                            Game.DrawString(ctx, x * 80 + 20, y * 70 + 78, "push", images);
                            Game.DrawString(ctx, x * 80 + 20, y * 70 + 88, "fire", images);
                            Game.DrawString(ctx, x * 80 + 28, y * 70 + 98, "!!", images);
                        }
                    }
                }
            }

            if (startTick < 600)
            {
                Game.DrawString(ctx, 320 - tick % (helpText.Length * 8 + 320), 192, helpText, assets.Alpha[1]);
            }
        }

        protected abstract bool AddPlayer(IController controller);

        protected abstract void Start();

        public virtual void Update()
        {
            tick++;

            List<IController> toRemove = new List<IController>();
            foreach (IController controller in unjoinedControllers)
            {
                if (controller.IsKeyDown(PlayerKeys.Bomb))
                {
                    if (AddPlayer(controller))
                    {
                        assets.Sounds.Addplayer.Play();

                        toRemove.Add(controller);
                    }
                }
            }

            foreach (IController controller in toRemove)
            {
                controller.Reset();
                controller.Update();
                unjoinedControllers.Remove(controller);
                joinedControllers.Add(controller);
            }

            if (Controller.IsKeyDown(controllers, PlayerKeys.StartGame) ||
                Controller.IsKeyDown(joinedControllers, PlayerKeys.Bomb))
            {
                Start();
            }

            if (startTick == -1)
            {
                if (Players.Count >= 1)
                {
                    startTick = 0;
                }
            }
            else
            {
                startTick++;
            }
        }

        public virtual void DrawHighDPI(SpriteBatch ctx, Rectangle rect, float scale, int graphicScale)
        {
        }
    }
}
