using System.Text;

namespace Sharpl.Types.Core;

public class UserType : Type<object>
{
    private readonly Value cons;

    public UserType(string name, AnyType[] parents, Value cons) : base(name, parents) {
        this.cons = cons;
    }

    public override void Call(VM vm, Stack stack, int arity, Loc loc)
    {
        cons.Call(vm, stack, arity, vm.NextRegisterIndex, true, loc);
        if (stack.TryPop(out var v)) { stack.Push(this, v.Data); }
        else throw new EvalError("Constructor didn't return a value", loc);
    }

    public override void Call(VM vm, Stack stack, Value target, int arity, int registerCount, bool eval, Loc loc) =>
        Parents[1].Call(vm, stack, target, arity, registerCount, eval, loc);

    public override void Dump(Value value, VM vm, StringBuilder result)
    {
        result.Append($"({Name} ");
        Parents[1].Dump(value, vm, result);
        result.Append(')');
    }
}
