using System.Collections.Generic;
using System.IO;

namespace MrBoom.NetworkProtocol.Messages
{
    public interface IMessage
    {
        void ReadFrom(BinaryReader reader);
        void WriteTo(BinaryWriter writer);
    }

    public interface IKeyedMessage<KeyT> : IMessage
    {
        KeyT Key { set; get; }
    }

    public interface IKeyedMessageCollection<KeyT, ValueT>
        where ValueT : IKeyedMessage<KeyT>, IMessage
    {
        IList<ValueT> Children { get; }
    }
}
