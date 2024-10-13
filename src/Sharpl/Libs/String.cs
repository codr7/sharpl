using System.Text;
using System.Text.RegularExpressions;

namespace Sharpl.Libs;

public class String : Lib
{
    private static string Stringify(Value v, Loc loc) =>
        (v.Type == Core.Char) ? $"{v.CastUnbox(Core.Char)}" : v.Cast(Core.String, loc);

    public String() : base("string", null, [])
    {
        BindMethod("down", ["in"], (vm, target, arity, result, loc) =>
        {
            var s = vm.GetRegister(0, 0).Cast(Core.String, loc);
            vm.Set(result, Value.Make(Core.String, s.ToLower()));
        });

        BindMethod("join", ["sep"], (vm, target, arity, result, loc) =>
        {
            var sep = vm.GetRegister(0, 0);
            var res = new StringBuilder();

            for (var i = 1; i < arity; i++)
            {
                if (sep.Type != Core.Nil && res.Length > 0) { sep.Say(vm, res); }
                vm.GetRegister(0, i).Say(vm, res);
            }

            vm.Set(result, Value.Make(Core.String, res.ToString()));
        });

        BindMethod("reverse", ["in"], (vm, target, arity, result, loc) =>
        {
            var s = vm.GetRegister(0, 0).Cast(Core.String, loc);
            char[] cs = s.ToCharArray();
            Array.Reverse(cs);
            vm.Set(result, Value.Make(Core.String, new string(cs)));
        });

        BindMethod("replace", ["in", "old", "new"], (vm, target, arity, result, loc) =>
        {
            var n = Stringify(vm.GetRegister(0, 2), loc);
            var o = Stringify(vm.GetRegister(0, 1), loc);
            vm.Set(result, Value.Make(Core.String, Regex.Replace(vm.GetRegister(0, 0).Cast(Core.String), o, n)));
        });

        BindMethod("split", ["in", "sep"], (vm, target, arity, result, loc) =>
        {
            var sep = Stringify(vm.GetRegister(0, 1), loc);

            var res = new Regex(sep).
                Split(Stringify(vm.GetRegister(0, 0), loc)).
                Select(s => Value.Make(Core.String, s)).
                ToArray();

            vm.Set(result, Value.Make(Core.Array, res));
        });

        BindMethod("up", ["in"], (vm, target, arity, result, loc) =>
        {
            var s = vm.GetRegister(0, 0).Cast(Core.String, loc);
            vm.Set(result, Value.Make(Core.String, s.ToUpper()));
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