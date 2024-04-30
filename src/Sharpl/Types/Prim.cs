namespace Sharpl.Types.Core;

using EmitArgs = LinkedList<Form>;

public class PrimType : MethodType
{
    public PrimType(string name) : base(name) { }

    public override void EmitCall(Loc loc, VM vm, Lib lib, Value target, EmitArgs args)
    {
        Form.Emit(args, vm, lib);
        vm.Emit(Ops.CallPrim.Make(loc, (Method)target.Data, args.Count));
    }
}
