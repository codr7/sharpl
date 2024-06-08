namespace Sharpl.Types.Core;

public class MacroType : Type<Macro>
{
    public MacroType(string name) : base(name) { }

    public override void EmitCall(Loc loc, VM vm, Lib lib, Value target, Form.Queue args)
    {
        target.Cast(this).Emit(loc, vm, lib, args);
    }
}