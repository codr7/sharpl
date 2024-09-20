using Sharpl.Iters.Core;
using Sharpl.Types.Core;
using System.Net.Sockets;
using System.Text;
using System.Threading.Channels;

namespace Sharpl.Types.Net;

public class ServerType : Type<TcpListener>, CloseTrait, IterTrait
{
    public ServerType(string name) : base(name) { }
    public void Close(Value target) => target.Cast(this).Stop();

    public Iter CreateIter(Value target, VM vm, Loc loc)
    {
        var s = target.Cast(this);
        var c = Channel.CreateUnbounded<Value>();

        Task.Run(async () =>
        {
            while (await s.AcceptTcpClientAsync() is TcpClient tc)
            {
                await c.Writer.WriteAsync(Value.Make(Libs.Net.Stream, tc.GetStream()));
            }
        });

        return new PipeItems(c);
    }

    public override void Dump(Value value, VM vm, StringBuilder result) =>
        result.Append($"(net/Server {vm.GetObjectId(value.Cast(this))})");
}