namespace Sharpl.Types.Core;

public class MetaType(string name, AnyType[] parents) : ComparableType<AnyType>(name, parents)
{
    public override void Call(VM vm, Value target, int arity, int registerCount, bool eval, Register result, Loc loc) =>
        target.Cast(this).Call(vm, arity, result, loc);
}