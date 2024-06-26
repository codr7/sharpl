namespace Sharpl.Libs;

using Sharpl.Types.Term;

using System.Text;

public class String : Lib
{
    public String() : base("string", null, [])
    {
        BindMethod("down", ["input"], (loc, target, vm, stack, arity) =>
        {
            var s = stack.Pop().Cast(Core.String);
            stack.Push(Core.String, s.ToLower());
        });

        BindMethod("join", ["separator"], (loc, target, vm, stack, arity) =>
        {
            stack.Reverse(arity);
            var sep = stack.Pop();
            var res = new StringBuilder();
            arity--;

            while (arity > 0) {
                if (sep.Type != Core.Nil && res.Length > 0) {
                    sep.Say(res);
                }

                stack.Pop().Say(res);
                arity--;
            }

            stack.Push(Core.String, res.ToString());
        });

        BindMethod("up", ["input"], (loc, target, vm, stack, arity) =>
        {
            var s = stack.Pop().Cast(Core.String);
            stack.Push(Core.String, s.ToUpper());
        });
    }
}