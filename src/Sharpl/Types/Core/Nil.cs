using System.Text;

namespace Sharpl.Types.Core;

public class NilType(string name, AnyType[] parents) : Type<bool>(name, parents), IterTrait
{
    public override bool Bool(Value value) => false;
    public Iter CreateIter(Value target, VM vm, Loc loc) => Iters.Core.Nil.Instance;
    public override void Dump(Value value, VM vm, StringBuilder result) => result.Append('_');
    public override string ToJson(Value value, Loc loc) => "null";
}
