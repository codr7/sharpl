namespace Sharpl.Types.Core;

public class MacroType : Type<Macro>
{
    public MacroType(string name) : base(name) { }

    public override void EmitCall(Loc loc, VM vm, Value target, Form.Queue args, int quoted)
    {
        if (quoted == 0) {
            target.Cast(this).Emit(loc, vm, args);
        } else {
            base.EmitCall(loc, vm, target, args, quoted);
        }
    }
}