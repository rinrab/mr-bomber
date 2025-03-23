// Copyright (c) Timofei Zhakov. All rights reserved.

using System;
using MrBoom.Core;
using MrBoom.Core.Sprites.Controllers;
using MrBoom.Core.Sprites.Providers;
using MrBoom.Core.Terrain;

namespace MrBoom
{
    public class ClientSpriteController : IServerGameEntity
    {
        private readonly IController controller;
        private readonly SpritePosition position;
        private readonly SpriteMovementController movementController;
        private readonly SpriteSpeedProvider speedProvider;
        private readonly IPlayerProxy proxy;

        public ClientSpriteController(IController controller,
                                      SpritePosition position,
                                      SpriteMovementController movementController,
                                      SpriteSpeedProvider speedProvider,
                                      IPlayerProxy proxy)
        {
            this.controller = controller;
            this.position = position;
            this.movementController = movementController;
            this.speedProvider = speedProvider;
            this.proxy = proxy;
        }

        private Directions? GetDirection()
        {
            if (controller.IsKeyDown(PlayerKeys.Up))
            {
                return Directions.Up;
            }
            else if (controller.IsKeyDown(PlayerKeys.Left))
            {
                return Directions.Left;
            }
            else if (controller.IsKeyDown(PlayerKeys.Right))
            {
                return Directions.Right;
            }
            else if (controller.IsKeyDown(PlayerKeys.Down))
            {
                return Directions.Down;
            }
            else
            {
                return null;
            }
        }

        public void ServerUpdate()
        {
            movementController.Move(GetDirection(), speedProvider.ProvideActualSpeed());

            if (Math.Abs(position.X - proxy.X) + Math.Abs(position.Y - proxy.Y) > 16)
            {
                position.MoveTo(proxy.X, proxy.Y);
            }

            if (controller.IsKeyDown(PlayerKeys.Bomb))
            {
                proxy.ToggleDropBomb();
            }

            if (controller.IsKeyDown(PlayerKeys.RcDitonate))
            {
                proxy.ToggleRemoteControl();
            }

            proxy.MoveTo(position.X, position.Y);
        }
    }
}
