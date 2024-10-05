using System.Text;

namespace Sharpl.Types.Core;

public class UserType : Type<object>
{
    private readonly Value cons;
    private readonly AnyType type;

    public UserType(string name, AnyType[] parents, Value cons) : base(name, parents) {
        this.cons = cons;
        type = parents.First(pt => pt is not UserTrait);
    }

    public override void Call(VM vm, Stack stack, int arity, Loc loc)
    {
        stack.Reverse(arity);
        cons.Call(vm, stack, arity, vm.NextRegisterIndex, true, loc);
        if (stack.TryPop(out var v)) { stack.Push(this, v.Data); }
        else throw new EvalError("Constructor didn't return a value", loc);
    }

    public override void Call(VM vm, Stack stack, Value target, int arity, int registerCount, bool eval, Loc loc) =>
        type.Call(vm, stack, target, arity, registerCount, eval, loc);

    public override void Dump(Value value, VM vm, StringBuilder result)
    {
        result.Append($"({Name} ");
        type.Dump(value, vm, result);
        result.Append(')');
    }
    public override void Say(Value value, VM vm, StringBuilder result) =>
        type.Say(value, vm, result);
}
