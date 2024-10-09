using Sharpl.Net;
using Sharpl.Types.Net;
using System.Net;
using System.Net.Sockets;
using System.Threading.Channels;

namespace Sharpl.Libs;

public class Net : Lib
{
    public static readonly ServerType Server = new ServerType("Server", [Core.Any]);
    public static readonly StreamType Stream = new StreamType("Stream", [Core.Any]);

    public Net() : base("net", null, [])
    {
        BindType(Server);
        BindType(Stream);

        BindMethod("connect", ["addr"], (vm, target, arity, result, loc) =>
        {
            var v = vm.GetRegister(0, 0).CastUnbox(Core.Pair, loc);
            var a = IPAddress.Parse(v.Item1.Cast(Core.String, loc));
            var c = new TcpClient();
            c.Connect(a, v.Item2.CastUnbox(Core.Int, loc));
            vm.Set(result, Value.Make(Stream, c.GetStream()));
        });

        BindMethod("accept", ["server"], (vm, target, arity, result, loc) =>
        {
            var s = vm.GetRegister(0, 0).Cast(Server, loc);
            var c = Channel.CreateUnbounded<Value>();

            Task.Run(async () =>
            {
                while (await s.AcceptTcpClientAsync() is TcpClient tc)
                    await c.Writer.WriteAsync(Value.Make(Stream, tc.GetStream()));
            });

            vm.Set(result, Value.Make(Core.Pipe, c));
        });

        BindMethod("listen", ["addr"], (vm, target, arity, result, loc) =>
        {
            var v = vm.GetRegister(0, 0).CastUnbox(Core.Pair, loc);
            var a = IPAddress.Parse(v.Item1.Cast(Core.String, loc));
            var s = new TcpListener(a, v.Item2.CastUnbox(Core.Int, loc));
            s.Start();
            vm.Set(result, Value.Make(Server, s));
        });

        BindMethod("stream-port", ["it"], (vm, target, arity, result, loc) =>
        {
            var s = vm.GetRegister(0, 0).Cast(Stream, loc);
            vm.Set(result, Value.Make(Core.Port, new StreamPort(s)));
        });
    }
}