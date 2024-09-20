using Sharpl.Net;
using Sharpl.Types.Net;
using System.Net;
using System.Net.Sockets;
using System.Threading.Channels;

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
            var v = stack.Pop().CastUnbox(Core.Pair, loc);
            var a = IPAddress.Parse(v.Item1.Cast(Core.String, loc));
            var c = new TcpClient();
            c.Connect(a, v.Item2.CastUnbox(Core.Int, loc));
            stack.Push(Stream, c.GetStream());
        });

        BindMethod("accept", ["server"], (loc, target, vm, stack, arity) =>
        {
            var s = stack.Pop().Cast(Server, loc);
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
            var v = stack.Pop().CastUnbox(Core.Pair, loc);
            var a = IPAddress.Parse(v.Item1.Cast(Core.String, loc));
            var s = new TcpListener(a, v.Item2.CastUnbox(Core.Int, loc));
            s.Start();
            stack.Push(Server, s);
        });

        BindMethod("stream-port", ["it"], (loc, target, vm, stack, arity) =>
        {
            var s = stack.Pop().Cast(Stream, loc);
            stack.Push(Core.Port, new StreamPort(s));
        });
    }
}