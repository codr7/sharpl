using System.Text;

namespace Sharpl.Types.Core;

public class IterType : Type<Iter>, IterTrait
{
    public IterType(string name) : base(name) { }
    public override void Dump(Value value, VM vm, StringBuilder result) => result.Append($"{value.Cast(this).Dump(vm)}");
    public Iter CreateIter(Value target, VM vm, Loc loc) => target.Cast(this);
}