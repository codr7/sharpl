namespace Sharpl.Types.Core;

public class UserMethodType : Type<UserMethod>
{
    public UserMethodType(string name) : base(name) { }

    public override void Call(VM vm, Stack stack, Value target, int arity, int registerCount, bool eval, Loc loc)
    {
        var startPC = vm.PC;
        var m = target.Cast(this);
        vm.CallUserMethod(loc, stack, m, new Value?[m.Args.Length], arity, registerCount);
        if (eval)
        {
            vm.EvalUntil(startPC, stack);
            vm.PC--;
        }
    }

    public override void EmitCall(VM vm, Value target, Form.Queue args, Loc loc)
    {
        var m = target.Cast(this);
        var arity = args.Count;
        var splat = args.IsSplat;
        if (!splat && arity < m.MinArgCount) { throw new EmitError($"Not enough arguments: {m}", loc); }
        if (splat) { vm.Emit(Ops.PushSplat.Make()); }
        var argMask = new Value?[Math.Max(arity, m.Args.Length)];
        var i = 0;

        foreach (var a in args)
        {
            if (a.GetValue(vm) is Value av)
            {
                if (av.Type == Libs.Core.Binding)
                {
                    var r = av.CastUnbox(Libs.Core.Binding);
                    av = Value.Make(Libs.Core.Binding, new Register(r.FrameOffset + 1, r.Index));
                }

                while (i < m.Args.Length && m.Args[i].Unzip)
                {
                    var p = av.CastUnbox(Libs.Core.Pair, loc);
                    argMask[i] = p.Item1;
                    i++;
                    av = p.Item2;
                }

                argMask[i] = av;
            }
            else
            {
                vm.Emit(a);
                var f = a;

                while (i < m.Args.Length && m.Args[i].Unzip) {
                    if (f is Forms.Pair pf)
                    {
                        f = pf.Right;
                        vm.Emit(Ops.Unzip.Make(loc));
                        i++;
                    }
                    else
                    {
                        throw new EmitError($"Expected pair: {f}", loc);
                    }
                }
            }

            i++;
        }

        args.Clear();
        vm.Emit(Ops.CallUserMethod.Make(m, argMask, splat, vm.NextRegisterIndex, loc));
    }
}