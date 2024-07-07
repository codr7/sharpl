using System.Text;

namespace Sharpl.Types.Core;

public class FixType : ComparableType<ulong>
{
    public FixType(string name) : base(name) { }

    public override bool Bool(Value value)
    {
        return Fix.Val(value.Cast(this)) != 0;
    }

    public override void Call(Loc loc, VM vm, Stack stack, int arity)
    {
        var v = stack.Pop().Cast(loc, Libs.Core.Int);
        var e = stack.Pop().Cast(loc, Libs.Core.Int);
        stack.Push(Value.Make(Libs.Core.Fix, Fix.Make((byte)e, v)));
    }

    public override void Dump(Value value, StringBuilder result)
    {
        result.Append(Fix.ToString(value.Cast(this)));
    }
}