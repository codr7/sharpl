namespace Sharpl.Types.Core;

using System.Text;

public class BitType(string name) : ComparableType<bool>(name)
{
    public override bool Bool(Value value) {
        return value.CastUnbox(this);
    }

    public override void Call(Loc loc, VM vm, Stack stack, int arity) {
        stack.Push(this, (bool)stack.Pop());
    }

    public override void Dump(Value value, StringBuilder result) {
        result.Append(value.CastUnbox(this) ? 'T' : 'F');
    }
}