// Copyright (c) Timofei Zhakov. All rights reserved.

using System;
using System.IO;

namespace MrBoom.NetworkProtocol
{
    public static class BinaryExtensions
    {
        public static Guid ReadGuid(this BinaryReader reader)
        {
            return new Guid(reader.ReadBytes(16));
        }

        public static void Write(this BinaryWriter writer, Guid guid)
        {
            writer.Write(guid.ToByteArray());
        }
    }
}
