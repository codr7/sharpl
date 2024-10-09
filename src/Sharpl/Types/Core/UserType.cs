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

    public override void Call(VM vm, int arity, Register result, Loc loc)
    {
        cons.Call(vm, arity, vm.NextRegisterIndex, true, result, loc);
        vm.Set(result, Value.Make(this, vm.Get(result).Data));
    }

    public override void Call(VM vm, Value target, int arity, int registerCount, bool eval, Register result, Loc loc) =>
        type.Call(vm, target, arity, registerCount, eval, result, loc);

    public override void Dump(VM vm, Value value, StringBuilder result)
    {
        result.Append($"({Name} ");
        type.Dump(vm, value, result);
        result.Append(')');
    }
    public override void Say(VM vm, Value value, StringBuilder result) => type.Say(vm, value, result);
}
