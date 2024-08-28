using System.Threading.Channels;

namespace Sharpl.Iters.Core;

public class PipeItems : BasicIter
{
    public readonly ChannelReader<Value> Source;

    public PipeItems(ChannelReader<Value> source)
    {
        Source = source;
    }

    public override Value? Next() {

        return Source.ReadAsync();
    }
}