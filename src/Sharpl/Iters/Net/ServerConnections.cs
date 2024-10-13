using System.Threading.Channels;

namespace Sharpl.Iters.Core;

public class ServerConnections : Iter
{
    public readonly ChannelReader<Value> Source;

    public ServerConnections(ChannelReader<Value> source)
    {
        Source = source;
    }

    public override bool Next(VM vm, Register result, Loc loc)
    {
        if (Source.TryRead(out var v))
        {
            vm.Set(result, v);
            return true;
        }

        return false;
    }
}