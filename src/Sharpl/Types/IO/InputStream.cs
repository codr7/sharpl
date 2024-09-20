using Sharpl.Iters.IO;
using Sharpl.Types.Core;
using System.Text;

namespace Sharpl.Types.IO;

public class InputStreamType : Type<TextReader>, IterTrait
{
    public InputStreamType(string name) : base(name) { }
    public override void Dump(Value value, VM vm, StringBuilder result) =>
        result.Append($"(InputStream {vm.GetObjectId(value.Cast(this))})");
    public Iter CreateIter(Value target, VM vm, Loc loc) => new StreamLines(target.Cast(this));
}