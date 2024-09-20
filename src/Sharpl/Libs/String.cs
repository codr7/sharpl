using System.Text;
using System.Text.RegularExpressions;

namespace Sharpl.Libs;

public class String : Lib
{
    public String() : base("string", null, [])
    {
        BindMethod("down", ["in"], (loc, target, vm, stack, arity) =>
        {
            var s = stack.Pop().Cast(Core.String);
            stack.Push(Core.String, s.ToLower());
        });

        BindMethod("join", ["sep"], (loc, target, vm, stack, arity) =>
        {
            stack.Reverse(arity);
            var sep = stack.Pop();
            var res = new StringBuilder();
            arity--;

            while (arity > 0)
            {
                if (sep.Type != Core.Nil && res.Length > 0) { sep.Say(vm, res); }
                stack.Pop().Say(vm, res);
                arity--;
            }

            stack.Push(Core.String, res.ToString());
        });

        BindMethod("reverse", ["in"], (loc, target, vm, stack, arity) =>
        {
            var s = stack.Pop().Cast(Core.String, loc);
            char[] cs = s.ToCharArray();
            Array.Reverse(cs);
            stack.Push(Core.String, new string(cs));
        });

        BindMethod("split", ["in", "sep"], (loc, target, vm, stack, arity) =>
        {
            var sep = stack.Pop().Cast(Core.String, loc);
            var res = new Regex(sep).
                Split(stack.Pop().Cast(Core.String, loc)).
                Select(s => Value.Make(Core.String, s)).
                ToArray();
            stack.Push(Core.Array, res);
        });

        BindMethod("up", ["in"], (loc, target, vm, stack, arity) =>
        {
            var s = stack.Pop().Cast(Core.String);
            stack.Push(Core.String, s.ToUpper());
        });

        BindMethod("replace", ["in", "old", "new"], (loc, target, vm, stack, arity) =>
        {
            var n = stack.Pop().Cast(Core.String, loc);
            var o = stack.Pop().Cast(Core.String, loc);
            var i = stack.Pop().Cast(Core.String, loc);
            o = o.Replace(" ", "\\s*");
            stack.Push(Value.Make(Core.String, Regex.Replace(i, o, n)));
        });
    }
}