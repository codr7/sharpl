namespace Sharpl.Types.Core;

public class MetaType : Type<AnyType>
{
    public MetaType(string name) : base(name) { }

 
    public override void Call(Loc loc, VM vm, Stack stack, Value target, int arity, bool recursive) {
        target.Cast(this).Call(loc, vm, stack, arity, recursive);
    }
}