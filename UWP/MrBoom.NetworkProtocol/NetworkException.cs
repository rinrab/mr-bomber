// Copyright (c) Timofei Zhakov. All rights reserved.

using System;

namespace MrBoom.NetworkProtocol.Messages
{
    public class NetworkException : Exception
    {
        public NetworkException() : base("Network exception occurred")
        {
        }
    }
}
