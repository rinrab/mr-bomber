// Copyright (c) Timofei Zhakov. All rights reserved.

using Microsoft.Xna.Framework.Graphics;

namespace MrBoom
{
    public class ClientSprite
    {
        private readonly Sprite proxy;

        public int X { get => proxy.X; }
        public int Y { get => proxy.Y; }

        public ClientSprite(Sprite proxy, Assets.MovingSpriteAssets animations)
        {
            this.proxy = proxy;
        }

        public void Update()
        {
            proxy.Update();
        }

        public void Draw(SpriteBatch ctx)
        {
            proxy.Draw(ctx);
        }
    }
}
