using System.IO;

namespace MrBoom.Common
{
    interface IMessage
    {
        void ReadFrom(BinaryReader reader);
        void WriteTo(BinaryWriter writer);
    }
}
