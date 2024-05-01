namespace Sharpl.Types.Core;

public class MethodType : Type<Method>
{
    public MethodType(string name) : base(name) { }

    public override void EmitCall(Loc loc, VM vm, Lib lib, Value target, Form.Queue args)
    {
        args.Emit(vm, lib);
        vm.Emit(Ops.CallMethod.Make(loc, target.Cast<Method>(), args.Count));
    }
}