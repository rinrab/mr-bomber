using System.IO;

namespace MrBoom.NetworkProtocol.Messages
{
    public interface IMessage
    {
        void ReadFrom(BinaryReader reader);
        void WriteTo(BinaryWriter writer);
    }
}
