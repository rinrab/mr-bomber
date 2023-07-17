﻿using Microsoft.Xna.Framework.Graphics;

namespace MrBoom
{
    public class Sprite : MovingSprite
    {
        public IController Controller;

        private Assets.AssetImage[][] animations;

        public Sprite(Terrain map, Assets.AssetImage[][] animations) : base(map)
        {
            //this.isPlayer = true;
            this.animations = animations;

            this.animateIndex = 0;
            this.frameIndex = 0;

            //this.playerKeys = { };

            this.speed = 1;

            //this.maxBoom = 1;
            //this.maxBombsCount = 1;
            //this.bombsPlaced = 0;

            //this.rcAllowed = false;

            //this.blinking = undefined;
            //this.blinkingSpeed = 15;

            //if (cheats.god)
            //{
            //    this.unplugin = 999999;
            //    this.blinking = 0;
            //    this.blinkingSpeed = 30;
            //}

            this.x = 1 * 16;
            this.y = 1 * 16;

            //const initialBonus = map.initialBonus;
            //if (initialBonus)
            //{
            //    if (initialBonus.includes(PowerUpType.Kick))
            //    {
            //        this.movingSprite.isHaveKick = true;
            //    }
            //}
            //if (mapIndex == 7)
            //{
            //    this.maxBoom = 8;
            //    this.maxBombsCount = 8;
            //}
        }

        public void Update()
        {
            this.Direction = MovingSprite.Directions.Right;

            Controller.Update();

            this.Direction = MovingSprite.Directions.None;
            if (this.Controller.Keys[PlayerKeys.Up])
            {
                this.Direction = Directions.Up;
            }
            else if (this.Controller.Keys[PlayerKeys.Left])
            {
                this.Direction = Directions.Left;
            }
            else if (this.Controller.Keys[PlayerKeys.Right])
            {
                this.Direction = Directions.Right;
            }
            else if (this.Controller.Keys[PlayerKeys.Down])
            {
                this.Direction = Directions.Down;
            }

            base.Update();
        }

        public void Draw(SpriteBatch ctx)
        {
            Assets.AssetImage[] animation = this.animations[this.animateIndex];
            Assets.AssetImage img = animation[frameIndex / 20 % animation.Length];
            //if (this.blinking % this.blinkingSpeed * 2 < this.blinkingSpeed)
            //{
            //    img = assets.boyGhost[this.animateIndex * 3 + frames[frameIndex % 4]];
            //}

            int x = this.x + 8 + 8 - img.Width / 2 / 2;
            int y = this.y + 16 - img.Height / 2;

            img.Draw(ctx, x, y);
        }
    }
}
