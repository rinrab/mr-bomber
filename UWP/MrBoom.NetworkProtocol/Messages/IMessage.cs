using System.IO;

namespace MrBoom.Common
{
    public interface IMessage
    {
        void ReadFrom(BinaryReader reader);
        void WriteTo(BinaryWriter writer);
    }
}
