namespace Sharpl.Types.Core;

public class MacroType(string name, AnyType[] parents) : Type<Macro>(name, parents)
{
    public override void EmitCall(VM vm, Value target, Form.Queue args, Register result, Loc loc)
    {
        target.CastUnbox(this).Emit(vm, args, result, loc);
        args.Emit(vm, result);
    }
}