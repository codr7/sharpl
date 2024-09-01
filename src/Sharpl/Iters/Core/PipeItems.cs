using System.Threading.Channels;

namespace Sharpl.Iters.Core;

public class PipeItems(ChannelReader<Value> Source) : BasicIter
{
    public override Value? Next() =>
        Task.Run<Value?>(async () => await Source.ReadAsync()).Result;
}