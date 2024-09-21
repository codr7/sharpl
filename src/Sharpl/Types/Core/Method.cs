namespace Sharpl.Types.Core;

public class MethodType : Type<Method>
{
    public MethodType(string name) : base(name) { }

    public override void Call(VM vm, Stack stack, Value target, int arity, int registerCount, bool eval, Loc loc) =>
        target.CastUnbox(this).Call(loc, vm, stack, arity);

    public override void EmitCall(VM vm, Value target, Form.Queue args, Loc loc)
    {
        var m = target.CastUnbox(this);
        var arity = args.Count;
        var splat = args.IsSplat;
        if (!splat && arity < m.MinArgCount) { throw new EmitError($"Not enough arguments: {m}", loc); }
        if (splat) { vm.Emit(Ops.PushSplat.Make()); }
        args.Emit(vm);
        vm.Emit(Ops.CallMethod.Make(loc, m, arity, splat));
    }
}
