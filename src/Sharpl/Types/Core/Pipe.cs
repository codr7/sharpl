using System.Text;
using System.Threading.Channels;
using Sharpl.Iters.Core;

namespace Sharpl.Types.Core;

public class PipeType : Type<Channel<Value>>, IterTrait
{
    public PipeType(string name) : base(name) { }
    public Iter CreateIter(Value target) => new PipeItems(target.Cast(this).Reader);
    public override void Dump(Value value, StringBuilder result) => result.Append($"(Pipe {value.Cast(this)})");
}