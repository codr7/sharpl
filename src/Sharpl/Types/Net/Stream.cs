using Sharpl.Types.Core;
using System.Net.Sockets;
using System.Text;

namespace Sharpl.Types.Net;

public class StreamType(string name, AnyType[] parents) : Type<NetworkStream>(name, parents), CloseTrait
{
    public void Close(Value target) => target.Cast(this).Close();
    public override void Dump(VM vm, Value value, StringBuilder result) =>
        result.Append($"(net/Stream {vm.GetObjectId(value.Cast(this))})");
}