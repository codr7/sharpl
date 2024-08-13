namespace Sharpl.Types.Core;

public class UserMethodType : Type<UserMethod>
{
    public UserMethodType(string name) : base(name) { }

    public override void Call(Loc loc, VM vm, Stack stack, Value target, int arity, int registerCount) {
        var m = target.Cast(this);
        vm.CallUserMethod(loc, stack, m, new Value?[m.Args.Length], registerCount);
    }

    public override void EmitCall(Loc loc, VM vm, Value target, Form.Queue args)
    {
        var m = target.Cast(this);
        var arity = args.Count;
        var splat = args.IsSplat;
        if (!splat && arity < m.MinArgCount) { throw new EmitError(loc, $"Not enough arguments: {m}"); }
        if (splat) { vm.Emit(Ops.PushSplat.Make()); }
        var argMask = new Value?[arity];
        var i = 0;
        
        foreach (var a in args) {
            if (a.GetValue(vm) is Value av) {
                if (av.Type == Libs.Core.Binding) {
                    var r = av.CastUnbox(Libs.Core.Binding);
                    av = Value.Make(Libs.Core.Binding, new Register(r.FrameOffset+1, r.Index));
                }

                argMask[i] = av; 
            }
            else { vm.Emit(a); }
            i++;
        }

        args.Clear();
        vm.Emit(Ops.CallUserMethod.Make(loc, m, argMask, splat, vm.NextRegisterIndex));
    }
}