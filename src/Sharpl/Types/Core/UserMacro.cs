namespace Sharpl.Types.Core;

public class UserMacroType : Type<UserMethod>
{
    public UserMacroType(string name) : base(name) { }

    public override void EmitCall(Loc loc, VM vm, Value target, Form.Queue args, int quoted)
    {
        var stack = new Stack();

        foreach (var f in args) {
            if (vm.Eval(f, quoted+1) is Value av) {
                stack.Push(av);
            }
        }

#pragma warning disable CS8629 
        vm.Eval((int)target.Cast(this).StartPC, stack);
#pragma warning restore CS8629

        args.Clear();

        if (stack.Pop() is Value rv) {
            rv.Unquote(loc, vm, args);
        }


    }
}