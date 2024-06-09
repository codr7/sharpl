namespace Sharpl.Types.Core;

public class MethodType : Type<Method>
{
    public MethodType(string name) : base(name) { }

    public override void EmitCall(Loc loc, VM vm, Env env, Value target, Form.Queue args)
    {
        var m = target.Cast(this);
        var arity = args.Count;

        if (arity < m.Args.Length)
        {
            throw new EmitError(loc, $"Not enough arguments: {m}");
        }

        args.Emit(vm, env);
        vm.Emit(Ops.CallMethod.Make(loc, m, arity));
    }
}