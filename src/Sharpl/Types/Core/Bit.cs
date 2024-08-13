using System.Text;

namespace Sharpl.Types.Core;

public class BitType(string name) : ComparableType<bool>(name)
{
    public override bool Bool(Value value) => value.CastUnbox(this);

    public override void Call(Loc loc, VM vm, Stack stack, int arity) => stack.Push(this, (bool)stack.Pop());

    public override void Dump(Value value, StringBuilder result) => result.Append(value.CastUnbox(this) ? 'T' : 'F');
}