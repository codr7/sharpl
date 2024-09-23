using System.Threading.Channels;

namespace Sharpl.Iters.Core;

public class PipeItems(ChannelReader<Value> Source) : Iter
{
    public override Value? Next(VM vm, Loc loc) =>
        Task.Run<Value?>(async () => await Source.ReadAsync()).Result;
}