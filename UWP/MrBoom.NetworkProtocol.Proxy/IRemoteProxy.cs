// Copyright (c) Timofei Zhakov. All rights reserved.

using MrBoom.NetworkProtocol.Messages;

namespace MrBoom.NetworkProtocol.Proxy
{
    public interface IRemoteProxy
    {
        void SetIncomingMessage(IMessage message);
        IMessage GetOutcomingMessage();
    }
}
