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
            var s = stack.Pop().Cast(loc, Core.String);
            char[] cs = s.ToCharArray();
            Array.Reverse(cs);
            stack.Push(Core.String, new string(cs));
        });

        BindMethod("up", ["in"], (loc, target, vm, stack, arity) =>
        {
            var s = stack.Pop().Cast(Core.String);
            stack.Push(Core.String, s.ToUpper());
        });

        BindMethod("replace", ["in", "old", "new"], (loc, target, vm, stack, arity) =>
        {
            var n = stack.Pop().Cast(loc, Core.String);
            var o = stack.Pop().Cast(loc, Core.String);
            var i = stack.Pop().Cast(loc, Core.String);
            o = o.Replace(" ", "\\s*");
            stack.Push(Value.Make(Core.String, Regex.Replace(i, o, n)));
        });
    }
}