using Sharpl.Iters.IO;
using Sharpl.Types.Core;
using System.Text;

namespace Sharpl.Types.IO;

public class InputStreamType(string name, AnyType[] parents) : 
    Type<TextReader>(name, parents), IterTrait
{
    public override void Dump(VM vm, Value value, StringBuilder result) =>
        result.Append($"(InputStream {vm.GetObjectId(value.Cast(this))})");
    public Iter CreateIter(Value target, VM vm, Loc loc) => new StreamLines(target.Cast(this));
}