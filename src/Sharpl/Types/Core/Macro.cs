namespace Sharpl.Types.Core;

public class MacroType : Type<Macro>
{
    public MacroType(string name) : base(name) { }

    public override void EmitCall(VM vm, Value target, Form.Queue args, Loc loc)
    {
        target.CastUnbox(this).Emit(vm, args, loc);
        args.Emit(vm);
    }
}