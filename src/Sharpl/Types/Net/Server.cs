using System.Net.Sockets;
using System.Text;
using System.Threading.Channels;
using Sharpl.Iters.Core;
using Sharpl.Types.Core;

namespace Sharpl.Types.Net;

public class ServerType : Type<TcpListener>, CloseTrait, IterTrait
{
    public ServerType(string name) : base(name) { }
    public void Close(Value target) => target.Cast(this).Stop();
    
    public Iter CreateIter(Value target)
    {
        var s = target.Cast(this);
        var c = Channel.CreateUnbounded<Value>();

        Task.Run(async () =>
        {
            if (await s.AcceptTcpClientAsync() is TcpClient tc)
            {
                await c.Writer.WriteAsync(Value.Make(Libs.Net.Stream, tc.GetStream()));
            }
        });

        return new PipeItems(c);
    }

    public override void Dump(Value value, StringBuilder result) => result.Append($"(Server {value.Cast(this)})");
}