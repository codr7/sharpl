namespace Sharpl.Types.Core;

public class MethodType : Type<Method>
{
    public MethodType(string name) : base(name) { }

    public override void Call(Loc loc, VM vm, Stack stack, Value target, int arity, int registerCount) {
        target.Cast(this).Call(loc, vm, stack, arity);
    }

    public override void EmitCall(Loc loc, VM vm, Value target, Form.Queue args)
    {
        var m = target.Cast(this);
        var arity = args.Count;
        var splat = args.IsSplat;
        
        if (!splat && arity < m.MinArgCount)
        {
            throw new EmitError(loc, $"Not enough arguments: {m}");
        }

        if (splat) {
            vm.Emit(Ops.PushSplat.Make());
        }

        args.Emit(vm);
        vm.Emit(Ops.CallMethod.Make(loc, m, arity, splat));
    }
}
