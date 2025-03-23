// Copyright (c) Timofei Zhakov. All rights reserved.

using System.Collections.Generic;
using MrBoom.Core.Service;
using MrBoom.Core.Sprites.Interface;

namespace MrBoom.Core.Sprites.Controllers
{
    public class SpriteHealthController : IHealthProvider, IPowerUpHandler
    {
        private readonly SpriteAnimationController animationController;
        private readonly BomberServiceProvider serviceProvider;

        public bool IsDie { get; protected set; }
        public bool IsAlive => !IsDie;

        public int LifeCount { get; protected set; }

        public int Unplugin { get; protected set; }
        public virtual bool HasUnplugin => Unplugin > 0;

        public SpriteHealthController(SpriteAnimationController animationController, BomberServiceProvider serviceProvider)
        {
            IsDie = false;
            this.animationController = animationController;
            this.serviceProvider = serviceProvider;
        }

        public void PickExtraLife()
        {
            LifeCount++;
        }

        public void PickUnplugin()
        {
            Unplugin = 165;
        }

        private IEnumerable<IDeathHandler> GetDeathHandlers()
        {
            return serviceProvider.EnumerateServices<IDeathHandler>();
        }

        private void KillInternal()
        {
            IsDie = true;
            animationController.SetAnimation(4);
            Unplugin = 0;
        }

        public void Kill()
        {
            KillInternal();

            foreach (IDeathHandler handler in GetDeathHandlers())
            {
                handler.OnDied(true);
            }
        }

        public virtual void Damage()
        {
            if (LifeCount > 0)
            {
                LifeCount--;
                PickUnplugin();

                foreach (IDeathHandler handler in GetDeathHandlers())
                {
                    handler.OnDamaged();
                }
            }
            else if (IsAlive)
            {
                KillInternal();

                foreach (IDeathHandler handler in GetDeathHandlers())
                {
                    handler.OnDied(false);
                }
            }
        }

        public PowerUpPickResult PickPowerUp(PowerUpType powerUpType)
        {
            if (powerUpType == PowerUpType.Life)
            {
                PickExtraLife();
                return PowerUpPickResult.Pick;
            }
            else if (powerUpType == PowerUpType.Shield)
            {
                PickUnplugin();
                return PowerUpPickResult.Pick;
            }
            else
            {
                return PowerUpPickResult.Skip;
            }
        }
    }
}
