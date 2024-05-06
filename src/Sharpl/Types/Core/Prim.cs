namespace Sharpl.Types.Core;

public class PrimType : MethodType
{
    public PrimType(string name) : base(name) { }

    public override void EmitCall(Loc loc, VM vm, Lib lib, Value target, Form.Queue args)
    {
        args.Emit(vm, lib);
        vm.Emit(Ops.CallPrim.Make(loc, target.Cast(this), args.Count));
    }
}
