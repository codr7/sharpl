namespace Sharpl.Types.Core;

public class MethodType(string name, AnyType[] parents) : Type<Method>(name, parents)
{
    public override void Call(VM vm, Stack stack, Value target, int arity, int registerCount, bool eval, Loc loc) =>
        target.CastUnbox(this).Call(vm, stack, arity, loc);

    public override void EmitCall(VM vm, Value target, Form.Queue args, Loc loc)
    {
        var m = target.CastUnbox(this);
        var arity = args.Count;
        var splat = args.IsSplat;
        if (!splat && arity < m.MinArgCount) { throw new EmitError($"Not enough arguments: {m}", loc); }
        if (splat) { vm.Emit(Ops.PushSplat.Make()); }
        args.Emit(vm);
        vm.Emit(Ops.CallMethod.Make(m, arity, splat, loc));
    }
}
