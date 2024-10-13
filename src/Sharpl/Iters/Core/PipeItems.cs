using System.Threading.Channels;

namespace Sharpl.Iters.Core;

public class PipeItems(ChannelReader<Value> Source) : Iter
{
    public override bool Next(VM vm, Register result, Loc loc)
    {
        if (Task.Run<Value?>(async () => await Source.ReadAsync()).Result is Value v)
        {
            vm.Set(result, v);
            return true;
        }
        
        return false;
    }
}