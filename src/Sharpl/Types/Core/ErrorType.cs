using System.Text;

namespace Sharpl.Types.Core;

public class ErrorType(string name, AnyType[] parents) : Type<UserError>(name, parents) {
    public override void Call(VM vm, Stack stack, int arity, Loc loc)
    {
        var res = new StringBuilder();

        while (arity > 0)
        {
            stack.Pop().Say(vm, res);
            arity--;
        }

        stack.Push(Libs.Core.String, res.ToString());
    }
}