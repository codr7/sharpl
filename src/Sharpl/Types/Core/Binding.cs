namespace Sharpl.Types.Core;

public class BindingType : Type<Binding>
{
    public BindingType(string name) : base(name) { }
    
    public override void EmitCall(Loc loc, VM vm, Value target, Form.Queue args) {
        var arity = args.Count;
        args.Emit(vm);
        var v = target.Cast(this);
        vm.Emit(Ops.GetRegister.Make(v.FrameOffset, v.Index));
        vm.Emit(Ops.CallIndirect.Make(loc, arity));
    }
    
    public override void EmitId(Loc loc, VM vm, Value target, Form.Queue args)
    {
        var v = target.Cast(this);
        vm.Emit(Ops.GetRegister.Make(v.FrameOffset, v.Index));
    }
}
