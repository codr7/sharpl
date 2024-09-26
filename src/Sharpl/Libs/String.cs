using System.Text;
using System.Text.RegularExpressions;

namespace Sharpl.Libs;

public class String : Lib
{
    private static string Stringify(Value v, Loc loc) =>
        (v.Type == Core.Char) ? $"{v.CastUnbox(Core.Char)}" : v.Cast(Core.String, loc);

    public String() : base("string", null, [])
    {
        BindMethod("down", ["in"], (vm, stack, target, arity, loc) =>
        {
            var s = stack.Pop().Cast(Core.String, loc);
            stack.Push(Core.String, s.ToLower());
        });

        BindMethod("join", ["sep"], (vm, stack, target, arity, loc) =>
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

        BindMethod("reverse", ["in"], (vm, stack, target, arity, loc) =>
        {
            var s = stack.Pop().Cast(Core.String, loc);
            char[] cs = s.ToCharArray();
            Array.Reverse(cs);
            stack.Push(Core.String, new string(cs));
        });

        BindMethod("replace", ["in", "old", "new"], (vm, stack, target, arity, loc) =>
        {
            var n = Stringify(stack.Pop(), loc);
            var o = Stringify(stack.Pop(), loc);
            stack.Push(Core.String, Regex.Replace(stack.Pop().Cast(Core.String), o, n));
        });

        BindMethod("split", ["in", "sep"], (vm, stack, target, arity, loc) =>
        {
            var sep = Stringify(stack.Pop(), loc);

            var res = new Regex(sep).
                Split(Stringify(stack.Pop(), loc)).
                Select(s => Value.Make(Core.String, s)).
                ToArray();

            stack.Push(Core.Array, res);
        });

        BindMethod("up", ["in"], (vm, stack, target, arity, loc) =>
        {
            var s = stack.Pop().Cast(Core.String, loc);
            stack.Push(Core.String, s.ToUpper());
        });
    }

    protected override void OnInit(VM vm)
    {
        Import(vm.CoreLib);

        vm.Eval("""
          (^strip [in it]
            (replace in it ""))

          (^trim [in]
            (strip in "\s*"))
        """);
    }
}