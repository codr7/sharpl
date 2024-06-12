namespace Sharpl.Types.Core;

public class MethodType : Type<Method>
{
    public MethodType(string name) : base(name) { }

    public override void Call(Loc loc, VM vm, Stack stack, Value target, int arity) {
        target.Cast(this).Call(loc, vm, stack, arity);
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
        vm.Emit(Ops.CallMethod.Make(loc, m, arity));
    }
}