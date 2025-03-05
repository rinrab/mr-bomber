// Copyright (c) Timofei Zhakov. All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using MrBoom.NetworkProtocol.Messages;

namespace MrBoom.NetworkProtocol.Proxy
{
    public interface IRouterChildrenFactory<KeyT>
    {
        IRemoteProxy CreateChild(KeyT key);
    }

    public class Router<KeyT>
    {
        private readonly IRouterChildrenFactory<KeyT> childrenFactory;
        private readonly IDictionary<KeyT, IRemoteProxy> children;

        public Router(IRouterChildrenFactory<KeyT> childrenFactory)
        {
            this.childrenFactory = childrenFactory;
            children = new Dictionary<KeyT, IRemoteProxy>();
        }

        public void SetIncomingMessage(IKeyedMessageCollection<KeyT, IKeyedMessage<KeyT>> message)
        {
            foreach (IKeyedMessage<KeyT> msg in message.Children)
            {
                if (children.TryGetValue(msg.Key, out IRemoteProxy child))
                {
                    child.SetIncomingMessage(msg);
                }
                else
                {
                    IRemoteProxy newChild = childrenFactory.CreateChild(msg.Key);
                    children.Add(msg.Key, newChild);
                    newChild.SetIncomingMessage(msg);
                }
            }
        }

        public IMessage GetOutcomingMessage()
        {
            throw new NotImplementedException();
        }
    }
}
