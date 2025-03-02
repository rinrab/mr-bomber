// Copyright (c) Timofei Zhakov. All rights reserved.

namespace MrBoom.Core
{
    public class ClientInfoFake : IClientInfo
    {
        public bool Equals(IClientInfo other)
        {
            return true;
        }
    }
}
