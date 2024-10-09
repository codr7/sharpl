using System.Text;

namespace Sharpl.Types.Core;

public class ErrorType(string name, AnyType[] parents) : Type<EvalError>(name, parents)
{
    public override void Call(VM vm, int arity, Register result, Loc loc)
    {
        var res = new StringBuilder();
        for (var i = 0; i < arity; i++) vm.GetRegister(0, i).Say(vm, res);
        vm.Set(result, Value.Make(Libs.Core.String, res.ToString()));
    }
}