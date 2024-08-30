using System.Net;
using System.Net.Sockets;
using System.Threading.Channels;

namespace Sharpl.Libs;

public class Json : Lib
{
    public Json() : base("json", null, [])
    {

        BindMethod("encode", ["value"], (loc, target, vm, stack, arity) =>
        {
            stack.Push(Core.String, stack.Pop().ToJson(loc));
        });
    }
}