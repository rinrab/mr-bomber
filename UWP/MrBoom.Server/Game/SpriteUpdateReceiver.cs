// Copyright (c) Timofei Zhakov. All rights reserved.

using MrBoom.Core.Sprites.Controllers;
using MrBoom.NetworkProtocol.Messages;

namespace MrBoom.Server.Game
{
    public class SpriteUpdateReceiver
    {
        private readonly SpritePosition position;
        private readonly SpriteBombController bombController;

        public SpriteUpdateReceiver(SpritePosition position,
                                    SpriteBombController bombController)
        {
            this.position = position;
            this.bombController = bombController;
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
                bombController.DropBomb();
            }

            if (message.RemoteControl)
            {
                bombController.ToggleRemoteControl();
            }
        }
    }
}
