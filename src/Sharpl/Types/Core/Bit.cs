using System.Text;

namespace Sharpl.Types.Core;

public class BitType(string name) : ComparableType<bool>(name)
{
    public override bool Bool(Value value) => value.CastUnbox(this);
    public override void Call(VM vm, Stack stack, int arity, Loc loc) => stack.Push(this, (bool)stack.Pop());
    public override void Dump(Value value, VM vm, StringBuilder result) => result.Append(value.CastUnbox(this) ? 'T' : 'F');
    public override string ToJson(Value value, Loc loc) => value.CastUnbox(this) ? "true" : "false";
}