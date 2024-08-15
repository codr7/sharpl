namespace Sharpl.Types.Core;

public class MacroType : Type<Macro>
{
    public MacroType(string name) : base(name) { }

    public override void EmitCall(Loc loc, VM vm, Value target, Form.Queue args) {
        target.CastUnbox(this).Emit(loc, vm, args);
        args.Emit(vm);
    }
}