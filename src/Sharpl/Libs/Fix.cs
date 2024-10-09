namespace Sharpl.Libs;

public class Fix : Lib
{
    public Fix() : base("fix", null, [])
    {
        BindMethod("to-int", ["value"], (vm, target, arity, result, loc) =>
            vm.Set(result, Value.Make(Core.Int, (int)Sharpl.Fix.Trunc(vm.GetRegister(0, 0).CastUnbox(Core.Fix, loc)))));
    }
}