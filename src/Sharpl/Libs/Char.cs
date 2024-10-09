namespace Sharpl.Libs;

public class Char : Lib
{
    public Char() : base("char", null, [])
    {
        BindMethod("digit", ["ch"], (vm, target, arity, result, loc) =>
            vm.Set(result, Value.Make(Core.Int, vm.GetRegister(0, 0).CastUnbox(Core.Char, loc) - '0')));

        BindMethod("down", ["in"], (vm, target, arity, result, loc) =>
        {
            var c = vm.GetRegister(0, 0).CastUnbox(Core.Char, loc);
            vm.Set(result, Value.Make(Core.Char, char.ToLower(c)));
        });

        BindMethod("is-digit", ["ch"], (vm, target, arity, result, loc) =>
            vm.Set(result, Value.Make(Core.Bit, char.IsDigit(vm.GetRegister(0, 0).CastUnbox(Core.Char, loc)))));

        BindMethod("up", ["in"], (vm, target, arity, result, loc) =>
        {
            var c = vm.GetRegister(0, 0).CastUnbox(Core.Char, loc);
            vm.Set(result, Value.Make(Core.Char, char.ToUpper(c)));
        });
    }
}