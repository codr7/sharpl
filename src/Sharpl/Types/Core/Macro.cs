namespace Sharpl.Types.Core;

public class MacroType(string name, AnyType[] parents) : Type<Macro>(name, parents)
{
    public override void EmitCall(VM vm, Value target, Form.Queue args, Loc loc)
    {
        target.CastUnbox(this).Emit(vm, args, loc);
        args.Emit(vm);
    }
}