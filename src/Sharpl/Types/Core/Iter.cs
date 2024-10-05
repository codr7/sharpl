using System.Text;

namespace Sharpl.Types.Core;

public class IterType(string name, AnyType[] parents) : Type<Iter>(name, parents), IterTrait
{
    public override void Dump(VM vm, Value value, StringBuilder result) => result.Append($"{value.Cast(this).Dump(vm)}");
    public Iter CreateIter(Value target, VM vm, Loc loc) => target.Cast(this);
}