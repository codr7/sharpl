namespace Sharpl.Types.Core;

using System.Text;

public class BitType : Type<bool>
{
    public BitType(string name) : base(name) { }

    public override bool Bool(Value value) {
        return value.Cast(this);
    }

    public override void Call(Loc loc, VM vm, Stack stack, int arity, bool recursive) {
        stack.Push(this, (bool)stack.Pop());
    }

    public override void Dump(Value value, StringBuilder result) {
        result.Append(value.Cast(this) ? 'T' : 'F');
    }
}