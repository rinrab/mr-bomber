// Copyright (c) Timofei Zhakov. All rights reserved.

using System;
using MrBoom.Core;

namespace MrBoom.NetworkProtocol.Proxy
{
    // IClientInfo backed by a guid
    public class ClientInfoGuid : IClientInfo
    {
        private readonly Guid clientId;

        public ClientInfoGuid(Guid clientId)
        {
            this.clientId = clientId;
        }

        public bool Equals(IClientInfo other)
        {
            if (other is ClientInfoGuid networkClientInfo)
            {
                return networkClientInfo.clientId == clientId;
            }
            else
            {
                return false;
            }
        }

        public Guid GetGuid()
        {
            return clientId;
        }
    }
}
