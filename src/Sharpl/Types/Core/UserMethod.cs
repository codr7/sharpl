namespace Sharpl.Types.Core;

public class UserMethodType : Type<UserMethod>
{
    public UserMethodType(string name) : base(name) { }

    public override void Call(Loc loc, VM vm, Stack stack, Value target, int arity, int registerCount)
    {
        vm.CallUserMethod(loc, stack, target.Cast(this), arity, registerCount);
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

        args.Emit(vm, 0);
        vm.Emit(Ops.CallUserMethod.Make(loc, m, arity, splat, vm.NextRegisterIndex));
    }
}