namespace Sharpl.Types.Core;

public class BindingType : Type<Binding>
{
    public BindingType(string name) : base(name) { }

    public override void EmitId(Loc loc, VM vm, Lib lib, Value target, Form.Queue args)
    {
        var v = target.Cast(this);
        vm.Emit(Ops.GetRegister.Make(v.FrameOffset, v.Index));
    }
}
