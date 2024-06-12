namespace Sharpl.Types.Core;

public class UserMethodType : Type<UserMethod>
{
    public UserMethodType(string name) : base(name) { }

    public override void Call(Loc loc, VM vm, Stack stack, Value target, int arity) {
        vm.Call(loc, target.Cast(this), arity);
    }

    public override void EmitCall(Loc loc, VM vm, Value target, Form.Queue args)
    {
        var m = target.Cast(this);
        var arity = args.Count;

        if (arity < m.Args.Length)
        {
            throw new EmitError(loc, $"Not enough arguments: {m}");
        }

        args.Emit(vm);
        vm.Emit(Ops.CallUserMethod.Make(loc, m, arity));
    }
}