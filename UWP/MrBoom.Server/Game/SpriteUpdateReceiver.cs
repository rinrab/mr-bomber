// Copyright (c) Timofei Zhakov. All rights reserved.

using MrBoom.Core.Sprites;
using MrBoom.NetworkProtocol.Messages;

namespace MrBoom.Server.Game
{
    public class SpriteUpdateReceiver
    {
        private readonly SpritePosition position;
        private readonly PlayerController controller;

        public SpriteUpdateReceiver(SpritePosition position,
                                    PlayerController controller)
        {
            this.position = position;
            this.controller = controller;
        }

        public void OnUpdateReceived(ClientPlayerUpdateMessage message)
        {
            if (Math.Abs(position.X - message.MoveToX) +
                Math.Abs(position.Y - message.MoveToY) < 8)
            {
                position.MoveTo(message.MoveToX, message.MoveToY);
            }

            if (message.DropBomb)
            {
                controller.DropBomb();
            }

            if (message.RemoteControl)
            {
                controller.RemoteDetonate();
            }
        }
    }
}
