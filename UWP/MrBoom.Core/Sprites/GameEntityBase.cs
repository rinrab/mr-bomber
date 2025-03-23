// Copyright (c) Timofei Zhakov. All rights reserved.

using MrBoom.Core.Service;

namespace MrBoom.Core.Sprites
{
    public class GameEntityBase : BomberServiceProvider, IServerGameEntity
    {
        public GameEntityBase()
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
