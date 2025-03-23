// Copyright (c) Timofei Zhakov. All rights reserved.

using System;
using MrBoom.Common;

namespace MrBoom.Core
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

        public override string ToString()
        {
            return clientId.ToFriendlyString();
        }
    }
}
