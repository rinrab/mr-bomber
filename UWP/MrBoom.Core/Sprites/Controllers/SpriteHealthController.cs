// Copyright (c) Timofei Zhakov. All rights reserved.

using MrBoom.Core.Sprites.Interface;

namespace MrBoom.Core.Sprites.Controllers
{
    public class SpriteHealthController : IHealthProvider, IPowerUpHandler
    {
        private readonly SpriteAnimationController animationController;

        public bool IsDie { get; protected set; }
        public bool IsAlive => !IsDie;

        public int LifeCount { get; protected set; }

        public int Unplugin { get; protected set; }
        public virtual bool HasUnplugin => Unplugin > 0;

        public SpriteHealthController(SpriteAnimationController animationController)
        {
            IsDie = false;
            this.animationController = animationController;
        }

        public void PickExtraLife()
        {
            LifeCount++;
        }

        public void PickUnplugin()
        {
            Unplugin = 165;
        }

        public void Kill()
        {
            IsDie = true;
            animationController.SetAnimation(0);
            Unplugin = 0;
        }

        public virtual void Damage()
        {
            if (LifeCount > 0)
            {
                LifeCount--;
                PickUnplugin();
            }
            else if (IsAlive)
            {
                Kill();
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
