using System.Threading.Channels;

namespace Sharpl.Iters.Core;

public class ServerConnections : Iter
{
    public readonly ChannelReader<Value> Source;

    public ServerConnections(ChannelReader<Value> source)
    {
        Source = source;
    }

    public override Value? Next(VM vm, Loc loc) => Source.TryRead(out var v) ? v : null;
}