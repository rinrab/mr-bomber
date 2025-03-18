// Copyright (c) Timofei Zhakov. All rights reserved.

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
