namespace Sharpl.Types.Core;

public class PrimType : MethodType
{
    public PrimType(string name) : base(name) { }

    public override void EmitCall(Loc loc, VM vm, Lib lib, Value target, EmitArgs args)
    {
        Form.Emit(args, vm, lib);
        vm.Emit(Ops.CallPrim.Make(loc, target.Cast<Method>(), args.Count));
    }
}
