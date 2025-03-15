// Copyright (c) Timofei Zhakov. All rights reserved.

namespace MrBoom.Core.Sprites
{
    public class SpriteBase : ServiceProvider, IServerGameEntity
    {
        public SpriteBase()
        {
        }

        public virtual void ServerUpdate()
        {
            foreach (IServerGameEntity service in EnumerateServices<IServerGameEntity>())
            {
                service.ServerUpdate();
            }
        }
    }
}
