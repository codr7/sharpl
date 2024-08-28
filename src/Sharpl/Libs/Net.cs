using System.Net;
using System.Net.Sockets;
using System.Threading.Channels;
using Sharpl.Types.Net;

namespace Sharpl.Libs;

public class Net : Lib
{
    public static readonly ServerType Server = new ServerType("Server");
    public static readonly StreamType Stream = new StreamType("Stream");

    public Net() : base("net", null, [])
    {
        BindType(Server);
        BindType(Stream);

        BindMethod("connect", ["addr"], (loc, target, vm, stack, arity) =>
        {
            var v = stack.Pop().CastUnbox(loc, Core.Pair);
            var a = IPAddress.Parse(v.Item1.Cast(loc, Core.String));
            var c = new TcpClient();
            c.Connect(a, v.Item2.CastUnbox(loc, Core.Int));
            stack.Push(Stream, c.GetStream());
        });

        BindMethod("accept", ["server"], (loc, target, vm, stack, arity) =>
        {
            var s = stack.Pop().Cast(loc, Server);
            var c = Channel.CreateUnbounded<Value>();

            Task.Run(async () =>
            {
                while (await s.AcceptTcpClientAsync() is TcpClient tc)
                {
                    await c.Writer.WriteAsync(Value.Make(Stream, tc.GetStream()));
                }
            });

            stack.Push(Core.Pipe, c);
        });

        BindMethod("listen", ["addr"], (loc, target, vm, stack, arity) =>
        {
            var v = stack.Pop().CastUnbox(loc, Core.Pair);
            var a = IPAddress.Parse(v.Item1.Cast(loc, Core.String));
            var s = new TcpListener(a, v.Item2.CastUnbox(loc, Core.Int));
            s.Start();
            stack.Push(Server, s);
        });
    }
}