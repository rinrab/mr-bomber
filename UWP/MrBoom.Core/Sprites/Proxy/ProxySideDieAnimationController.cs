// Copyright (c) Timofei Zhakov. All rights reserved.

using MrBoom.Core.Sprites.Controllers;
using MrBoom.Core.Sprites.Interface;

namespace MrBoom.Core.Sprites.Proxy
{
    public class ProxySideDieAnimationController : IServerGameEntity
    {
        private readonly IHealthProvider health;
        private readonly SpriteAnimationController animationController;

        public ProxySideDieAnimationController(IHealthProvider health,
                                               SpriteAnimationController animationController)
        {
            this.health = health;
            this.animationController = animationController;
        }

        public void ServerUpdate()
        {
            if (health.IsDie)
            {
                animationController.SetAnimation(4);
            }
        }
    }
}
