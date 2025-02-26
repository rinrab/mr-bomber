// Copyright (c) Timofei Zhakov. All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;

namespace MrBoom.NetworkProtocol.Messages
{
    public class NetworkException : Exception
    {
        public NetworkException() : base("Network exception occurred")
        {
        }
    }
}
