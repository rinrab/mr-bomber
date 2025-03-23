// Copyright (c) Timofei Zhakov. All rights reserved.

using MrBoom.Core.Terrain;
using MrBoom.NetworkProtocol.Messages;

namespace MrBoom.NetworkProtocol.Proxy
{
    public class PlayerProxy : SpriteProxy, IPlayerProxy, IRemoteProxy
    {
        private readonly int index;

        private int x;
        private int y;

        private bool dropBomb = false;
        private bool remoteControl = false;

        public PlayerProxy(int index)
        {
            this.index = index;
        }

        public void MoveTo(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public void SetDirection(Directions? direction)
        {
        }

        public void ToggleDropBomb()
        {
            dropBomb = true;
        }

        public void ToggleRemoteControl()
        {
            remoteControl = true;
        }

        public override void SetIncomingMessage(IMessage message)
        {
            dropBomb = false;
            remoteControl = false;

            base.SetIncomingMessage(message);
        }

        public override IMessage GetOutcomingMessage()
        {
            return new ClientPlayerUpdateMessage
            {
                Index = (byte)index,
                MoveToX = x,
                MoveToY = y,
                DropBomb = dropBomb,
                RemoteControl = remoteControl,
            };
        }
    }
}
