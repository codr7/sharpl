namespace Sharpl.Types.Core;

public class MetaType(string name, AnyType[] parents) : ComparableType<AnyType>(name, parents)
{
    public override void Call(VM vm, Stack stack, Value target, int arity, int registerCount, bool eval, Loc loc) =>
        target.Cast(this).Call(vm, stack, arity, loc);
}