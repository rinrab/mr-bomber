// Copyright (c) Timofei Zhakov. All rights reserved.

using System;

namespace MrBoom.Common
{
    public static class GuidExtensions
    {
        public static string ToFriendlyString(this Guid guid)
        {
            return guid.ToString("N").Substring(0, 7);
        }
    }
}
