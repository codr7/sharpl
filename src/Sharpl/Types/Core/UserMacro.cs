namespace Sharpl.Types.Core;

public class UserMacroType : Type<UserMethod>
{
    public UserMacroType(string name) : base(name) { }

    public override void EmitCall(Loc loc, VM vm, Value target, Form.Queue args)
    {
        var stack = new Stack();

        foreach (var f in args) {
            Console.WriteLine("EVAL MACRO ARG " + f);
            if (vm.Eval(f, 1) is Value av) {
                stack.Push(av);
            }
        }

#pragma warning disable CS8629 
        vm.Eval((int)target.Cast(this).StartPC, stack);
#pragma warning restore CS8629

        args.Clear();

        if (stack.Pop() is Value rv) {
            args.PushFirst(rv.Unquote(loc, vm));
        }


    }
}