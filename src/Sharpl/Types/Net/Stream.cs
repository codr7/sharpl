using System.Net.Sockets;
using System.Text;
using Sharpl.Types.Core;

namespace Sharpl.Types.Net;

public class StreamType : Type<NetworkStream>, CloseTrait
{
    public StreamType(string name) : base(name) { }
    public void Close(Value target) => target.Cast(this).Close();
    public override void Dump(Value value, StringBuilder result) => result.Append($"(Stream {value.Cast(this)})");
}