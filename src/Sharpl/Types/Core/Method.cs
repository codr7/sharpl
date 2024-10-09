namespace Sharpl.Types.Core;

public class MethodType(string name, AnyType[] parents) : Type<Method>(name, parents)
{
    public override void Call(VM vm, Value target, int arity, int registerCount, bool eval, Register result, Loc loc) =>
        target.CastUnbox(this).Call(vm, arity, result, loc);

    public override void EmitCall(VM vm, Value target, Form.Queue args, Register result, Loc loc)
    {
        var m = target.CastUnbox(this);
        var arity = args.Count;
        var splat = args.IsSplat;
        if (!splat && arity < m.MinArgCount) { throw new EmitError($"Not enough arguments: {m}", loc); }
        if (splat) { vm.Emit(Ops.PushSplat.Make()); }
        for (var i = 0; i < args.Count; i++) vm.Emit(args.Items[i], new Register(0, i));
        vm.Emit(Ops.CallMethod.Make(m, arity, splat, result, loc));
    }
}
