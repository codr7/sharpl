namespace Sharpl.Types.Core;

public class BindingType : Type<Binding>
{
    public BindingType(string name) : base(name) { }
    
    public override void EmitCall(Loc loc, VM vm, Value target, Form.Queue args) {
        var arity = args.Count;
        var splat = args.IsSplat;
        args.Emit(vm);
        var v = target.Cast(this);
        vm.Emit(Ops.CallRegister.Make(loc, v.FrameOffset, v.Index, arity, splat, vm.NextRegisterIndex));
    }
    
    public override void Emit(Loc loc, VM vm, Value target, Form.Queue args)
    {
        var v = target.Cast(this);
        vm.Emit(Ops.GetRegister.Make(v.FrameOffset, v.Index));
    }
}
