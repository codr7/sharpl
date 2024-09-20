using System.Text;

namespace Sharpl.Types.Core;

public class NilType : Type<bool>, IterTrait
{
    public NilType(string name) : base(name) { }
    public override bool Bool(Value value) => false;
    public Iter CreateIter(Value target, VM vm, Loc loc) => Iters.Core.Nil.Instance;
    public override void Dump(Value value, VM vm, StringBuilder result) =>  result.Append('_');
    public override string ToJson(Value value, Loc loc) => "null";
}
