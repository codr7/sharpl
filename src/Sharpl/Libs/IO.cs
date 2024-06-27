namespace Sharpl.Libs;

using Sharpl.Iters.IO;
using Sharpl.Types.IO;

using System.Text;

public class IO : Lib
{
    public static readonly ReadStreamType ReadStream = new ReadStreamType("ReadStream");

    public IO() : base("io", null, [])
    {
        BindType(ReadStream);

        Bind("IN", Value.Make(IO.ReadStream, Console.In));

        BindMethod("lines", ["in"], (loc, target, vm, stack, arity) =>
        {
            var s = stack.Pop().Cast(ReadStream);
            stack.Push(Value.Make(Core.Iter, new StreamLines(s)));
        });
    }
}