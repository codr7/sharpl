using Sharpl.Types.Core;
using System.Net.Sockets;
using System.Text;

namespace Sharpl.Types.Net;

public class StreamType : Type<NetworkStream>, CloseTrait
{
    public StreamType(string name) : base(name) { }
    public void Close(Value target) => target.Cast(this).Close();
    public override void Dump(Value value, VM vm, StringBuilder result) =>
        result.Append($"(net/Stream {vm.GetObjectId(value.Cast(this))})");
}