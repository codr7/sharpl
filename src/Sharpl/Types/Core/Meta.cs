namespace Sharpl.Types.Core;

public class MetaType : Type<AnyType>
{
    public MetaType(string name) : base(name) { }

    public override void Call(VM vm, Stack stack, Value target, int arity, int registerCount, bool eval, Loc loc) =>
        target.Cast(this).Call(vm, stack, arity, loc);
}