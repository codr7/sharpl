using System.Threading.Channels;

namespace Sharpl.Iters.Core;

public class ServerConnections : BasicIter
{
    public readonly ChannelReader<Value> Source;

    public ServerConnections(ChannelReader<Value> source)
    {
        Source = source;
    }

    public override Value? Next() => Source.TryRead(out var v) ? v : null;
}